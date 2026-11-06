using Products.API.Constants;
using Products.API.Http;
using Products.API.DTOs;
using Products.API.Exceptions;
using Products.API.Models;

namespace Products.API.Services;

public class ProductoService(OrdersClient ordersClient) : IProductoService
{
    private static readonly List<Producto> _productos = new();

    public Task<List<ProductoResponseDto>> ObtenerTodosAsync(string? categoria, string? nombre)
    {
        var query = _productos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(categoria))
        {
            query = query.Where(p => p.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            query = query.Where(p => p.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));
        }

        var resultado = query.Select(MapearAResponseDto).ToList();
        return Task.FromResult(resultado);
    }

    public Task<ProductoResponseDto> ObtenerPorIdAsync(Guid id)
    {
        var producto = _productos.FirstOrDefault(p => p.Id == id);

        if (producto is null)
            throw new NotFoundException(ErrorCodes.PRD_001, "Producto no encontrado.");

        return Task.FromResult(MapearAResponseDto(producto));
    }

    public Task<ProductoResponseDto> CrearAsync(CrearProductoDto dto)
    {
        var yaExiste = _productos.Any(p =>
            p.Nombre.Equals(dto.Nombre, StringComparison.OrdinalIgnoreCase) &&
            p.Categoria.Equals(dto.Categoria, StringComparison.OrdinalIgnoreCase));

        if (yaExiste)
            throw new BusinessRuleException(ErrorCodes.PRD_003, $"Ya existe un producto con ese nombre en la categoría '{dto.Categoria}'.");

        var producto = new Producto
        {
            Id = Guid.NewGuid(),
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Precio = dto.Precio,
            Stock = dto.Stock,
            Categoria = dto.Categoria,
            FechaCreacion = DateTime.UtcNow
        };

        _productos.Add(producto);

        return Task.FromResult(MapearAResponseDto(producto));
    }

    public Task<ProductoResponseDto> ActualizarAsync(Guid id, ActualizarProductoDto dto)
    {
        var producto = _productos.FirstOrDefault(p => p.Id == id);

        if (producto is null)
            throw new NotFoundException(ErrorCodes.PRD_001, "Producto no encontrado.");

        producto.Nombre = dto.Nombre;
        producto.Descripcion = dto.Descripcion;
        producto.Precio = dto.Precio;
        producto.Stock = dto.Stock;
        producto.Categoria = dto.Categoria;

        return Task.FromResult(MapearAResponseDto(producto));
    }

    public async Task EliminarAsync(Guid id)
    {
        var producto = _productos.FirstOrDefault(p => p.Id == id);

        if (producto is null)
            throw new NotFoundException(ErrorCodes.PRD_001, "Producto no encontrado.");

        // Regla PRD-004: Orders informa si hay órdenes Pendiente o Confirmada con este producto.
        if (await ordersClient.TieneOrdenesActivasAsync(id))
            throw new BusinessRuleException(ErrorCodes.PRD_004, "El producto tiene órdenes activas y no puede eliminarse.");

        _productos.Remove(producto);
    }

    private static ProductoResponseDto MapearAResponseDto(Producto p) => new()
    {
        Id = p.Id,
        Nombre = p.Nombre,
        Descripcion = p.Descripcion,
        Precio = p.Precio,
        Stock = p.Stock,
        Categoria = p.Categoria,
        FechaCreacion = p.FechaCreacion
    };
}