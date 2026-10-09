using Orders.API.DTOs;

namespace Orders.API.Services;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken ct);

    bool HasActiveOrders(Guid productoId);
}
