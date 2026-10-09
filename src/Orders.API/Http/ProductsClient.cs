using System.Net;
using System.Net.Http.Json;

namespace Orders.API.Http;

public record ProductInfo(Guid Id, string Nombre, decimal Precio, int Stock);

public class ProductsClient(IHttpClientFactory factory, ILogger<ProductsClient> logger)
{
    public async Task<ProductInfo?> GetByIdAsync(Guid productoId, CancellationToken ct)
    {
        var client = factory.CreateClient(HttpClients.Products);
        var response = await client.GetAsync($"/api/products/{productoId}", ct);

        if (response.StatusCode == HttpStatusCode.NotFound) return null;

        // Siempre se revisa el status antes de leer el body.
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Products respondio {StatusCode} al consultar el producto {ProductoId}",
                (int)response.StatusCode, productoId);

            throw new HttpRequestException($"Products.API respondio {(int)response.StatusCode}.");
        }

        return await response.Content.ReadFromJsonAsync<ProductInfo>(ct);
    }
}
