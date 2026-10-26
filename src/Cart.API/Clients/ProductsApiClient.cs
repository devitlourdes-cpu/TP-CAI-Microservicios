using System.Net;
using System.Net.Http.Json;

namespace Cart.API.Clients;

public class ProductsApiClient(HttpClient httpClient) : IProductsApiClient
{
    public async Task<ProductoDto?> ObtenerPorIdAsync(Guid id, CancellationToken ct)
    {
        var response = await httpClient.GetAsync($"/api/products/{id}", ct);

        // Si Products.API devuelve 404, no es un error nuestro: simplemente no existe.
        if (response.StatusCode == HttpStatusCode.NotFound) return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProductoDto>(cancellationToken: ct);
    }
}