using System.Net.Http.Json;

namespace Products.API.Http;

public record OrdenesActivasResponse(Guid ProductoId, bool TieneOrdenesActivas);

public class OrdersClient(HttpClient http, ILogger<OrdersClient> logger)
{
    public async Task<bool> TieneOrdenesActivasAsync(Guid productoId, CancellationToken ct = default)
    {
        var response = await http.GetAsync($"/api/orders/product/{productoId}/active", ct);

        // Si no se puede verificar, no se borra: el error sale como PRD-005.
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Orders respondio {StatusCode} al consultar ordenes activas del producto {ProductoId}",
                (int)response.StatusCode, productoId);
            throw new HttpRequestException($"Orders.API respondio {(int)response.StatusCode}.");
        }

        var resultado = await response.Content.ReadFromJsonAsync<OrdenesActivasResponse>(ct);
        return resultado?.TieneOrdenesActivas
            ?? throw new HttpRequestException("Orders.API devolvio una respuesta vacia.");
    }
}