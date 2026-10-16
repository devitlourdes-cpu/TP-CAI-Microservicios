using Orders.API.Constants;
using Orders.API.DTOs;
using Orders.API.Exceptions;
using Orders.API.Http;
using Orders.API.Models;

namespace Orders.API.Services;

public class OrderService(
    UsersClient usersClient,
    ProductsClient productsClient,
    ILogger<OrderService> logger) : IOrderService
{
    // Guardamos las ordenes en memoria hasta que este la libreria de la catedra.
    private readonly List<Order> _orders = [];
    private readonly Lock _candado = new();

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken ct)
    {
        if (!await usersClient.ExistsAsync(request.UsuarioId, ct))
            throw new NotFoundException(ErrorCodes.ORD_003, "Usuario no encontrado al crear la orden.");

        // Se suman las cantidades por producto: si el mismo producto viene repetido en
        // varios items, el stock tiene que alcanzar para el total y no para cada linea.
        var cantidadPedida = request.Items
            .GroupBy(i => i.ProductoId)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.Cantidad));

        var precios = new Dictionary<Guid, decimal>();

        foreach (var (productoId, cantidad) in cantidadPedida)
        {
            var producto = await productsClient.GetByIdAsync(productoId, ct)
                ?? throw new NotFoundException(ErrorCodes.ORD_004, "Producto no encontrado al crear la orden.");

            if (producto.Stock < cantidad)
                throw new BusinessRuleException(ErrorCodes.ORD_005,
                    $"Stock insuficiente para '{producto.Nombre}'. " +
                    $"Disponible: {producto.Stock}, solicitado: {cantidad}.", 422);

            precios[productoId] = producto.Precio;
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UsuarioId = request.UsuarioId,
            Items = [.. request.Items.Select(i => new OrderItem
            {
                ProductoId = i.ProductoId,
                Cantidad = i.Cantidad,
                PrecioUnitario = precios[i.ProductoId]
            })],
            Estado = OrderStates.Pendiente,
            FechaCreacion = DateTime.UtcNow
        };

        order.Total = order.Items.Sum(i => i.Cantidad * i.PrecioUnitario);

        lock (_candado)
        {
            _orders.Add(order);
        }

        logger.LogInformation("Orden creada. Id: {OrdenId}, usuario: {UsuarioId}, total: {Total}",
            order.Id, order.UsuarioId, order.Total);

        return Map(order);
    }

    public List<OrderResponse> GetAll(Guid? usuarioId)
    {
        lock (_candado)
        {
            return [.. _orders
                .Where(o => usuarioId is null || o.UsuarioId == usuarioId)
                .OrderByDescending(o => o.FechaCreacion)
                .Select(Map)];
        }
    }

    public OrderResponse GetById(Guid id)
    {
        lock (_candado)
        {
            var order = _orders.FirstOrDefault(o => o.Id == id)
                ?? throw new NotFoundException(ErrorCodes.ORD_001, "Orden no encontrada.");

            return Map(order);
        }
    }

    public bool HasActiveOrders(Guid productoId)
    {
        lock (_candado)
        {
            return _orders.Any(o =>
                OrderStates.Activos.Contains(o.Estado) &&
                o.Items.Any(i => i.ProductoId == productoId));
        }
    }

    private static OrderResponse Map(Order order) => new()
    {
        Id = order.Id,
        UsuarioId = order.UsuarioId,
        Items = [.. order.Items.Select(i => new OrderItemResponse
        {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.PrecioUnitario
        })],
        Total = order.Total,
        Estado = order.Estado,
        FechaCreacion = order.FechaCreacion
    };
}
