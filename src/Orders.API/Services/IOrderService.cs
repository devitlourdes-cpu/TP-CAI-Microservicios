using Orders.API.DTOs;

namespace Orders.API.Services;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken ct);

    List<OrderResponse> GetAll(Guid? usuarioId);

    OrderResponse GetById(Guid id);

    bool HasActiveOrders(Guid productoId);
}
