using System.Text.Json.Serialization;

namespace Cart.API.Clients;

// Solo traemos los campos que Cart.API necesita (no todo el producto).
public record ProductoDto(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("nombre")] string Nombre,
    [property: JsonPropertyName("precio")] decimal Precio,
    [property: JsonPropertyName("stock")] int Stock
);

public interface IProductsApiClient
{
    Task<ProductoDto?> ObtenerPorIdAsync(Guid id, CancellationToken ct);
}