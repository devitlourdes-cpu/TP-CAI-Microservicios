using Cart.API.Clients;
using Cart.API.Constants;
using Cart.API.DTOs;
using Cart.API.Exceptions;
using Cart.API.Models;

namespace Cart.API.Services;

public class CartService(IProductsApiClient productsApi) : ICartService
{
    // Un carrito por usuario, guardado en memoria.
    private readonly Dictionary<Guid, Carrito> _carritos = [];
    private readonly Lock _candado = new();

    public Task<CarritoResponseDto> ObtenerAsync(Guid userId)
    {
        lock (_candado)
        {
            var carrito = _carritos.GetValueOrDefault(userId)
                ?? throw new NotFoundException(ErrorCodes.CRT_001, "No hay carrito activo para este usuario.");

            return Task.FromResult(Mapear(carrito));
        }
    }

    public async Task<CarritoResponseDto> AgregarItemAsync(Guid userId, AgregarItemCarritoDto dto, CancellationToken ct)
    {
        // La llamada a Products.API va ANTES del lock: nunca se puede hacer "await"
        // dentro de un lock, y tampoco conviene tener el candado tomado mientras
        // esperamos una respuesta de otro servicio por la red.
        var producto = await productsApi.ObtenerPorIdAsync(dto.ProductoId, ct)
            ?? throw new NotFoundException(ErrorCodes.CRT_002, "El producto no existe.");

        if (producto.Stock < dto.Cantidad)
            throw new BusinessRuleException(ErrorCodes.CRT_003,
                $"Stock insuficiente. Disponible: {producto.Stock}, solicitado: {dto.Cantidad}.");

        lock (_candado)
        {
            if (!_carritos.TryGetValue(userId, out var carrito))
            {
                carrito = new Carrito { UsuarioId = userId };
                _carritos[userId] = carrito;
            }

            // Si el producto ya estaba en el carrito, se reemplaza la cantidad (no se suma).
            var item = carrito.Items.FirstOrDefault(i => i.ProductoId == dto.ProductoId);
            if (item is null)
            {
                carrito.Items.Add(new CarritoItem
                {
                    ProductoId = producto.Id,
                    NombreProducto = producto.Nombre,
                    PrecioUnitario = producto.Precio,
                    Cantidad = dto.Cantidad
                });
            }
            else
            {
                item.Cantidad = dto.Cantidad;
                item.PrecioUnitario = producto.Precio;
            }

            return Mapear(carrito);
        }
    }

    public async Task<CarritoResponseDto> ActualizarItemAsync(Guid userId, Guid productId, ActualizarItemCarritoDto dto, CancellationToken ct)
    {
        var producto = await productsApi.ObtenerPorIdAsync(productId, ct)
            ?? throw new NotFoundException(ErrorCodes.CRT_002, "El producto no existe.");

        if (producto.Stock < dto.Cantidad)
            throw new BusinessRuleException(ErrorCodes.CRT_003,
                $"Stock insuficiente. Disponible: {producto.Stock}, solicitado: {dto.Cantidad}.");

        lock (_candado)
        {
            var carrito = _carritos.GetValueOrDefault(userId)
                ?? throw new NotFoundException(ErrorCodes.CRT_001, "No hay carrito activo para este usuario.");

            var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productId)
                ?? throw new NotFoundException(ErrorCodes.CRT_002, "El producto no esta en el carrito.");

            item.Cantidad = dto.Cantidad;
            item.PrecioUnitario = producto.Precio;

            return Mapear(carrito);
        }
    }

    public Task QuitarItemAsync(Guid userId, Guid productId)
    {
        lock (_candado)
        {
            var carrito = _carritos.GetValueOrDefault(userId)
                ?? throw new NotFoundException(ErrorCodes.CRT_001, "No hay carrito activo para este usuario.");

            carrito.Items.RemoveAll(i => i.ProductoId == productId);
            return Task.CompletedTask;
        }
    }

    public Task VaciarAsync(Guid userId)
    {
        lock (_candado)
        {
            var carrito = _carritos.GetValueOrDefault(userId)
                ?? throw new NotFoundException(ErrorCodes.CRT_001, "No hay carrito activo para este usuario.");

            carrito.Items.Clear();
            return Task.CompletedTask;
        }
    }

    private static CarritoResponseDto Mapear(Carrito carrito) => new()
    {
        UsuarioId = carrito.UsuarioId,
        Items = carrito.Items.Select(i => new CarritoItemResponseDto
        {
            ProductoId = i.ProductoId,
            NombreProducto = i.NombreProducto,
            PrecioUnitario = i.PrecioUnitario,
            Cantidad = i.Cantidad
        }).ToList()
    };
}