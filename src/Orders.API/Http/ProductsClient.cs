using System.Net;
using System.Net.Http.Json;

namespace Orders.API.Http;

// Lo unico que Orders necesita de un producto: el nombre para el mensaje de error,
// el precio para el item y el stock para validar. Products devuelve mas campos.
public record ProductInfo(Guid Id, string Nombre, decimal Precio, int Stock);

// Consulta productos en Products.API, para validar ORD-004 y ORD-005.
public class ProductsClient(IHttpClientFactory factory, ILogger<ProductsClient> logger)
{
    // Devuelve null cuando el producto no existe.
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
