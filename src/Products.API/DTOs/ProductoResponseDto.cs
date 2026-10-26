namespace Products.API.DTOs;

public class ProductoResponseDto
{
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }
    /// <example>Notebook Dell XPS 15</example>
    public required string Nombre { get; set; }
    /// <example>Laptop 15 pulgadas, 32GB RAM</example>
    public string? Descripcion { get; set; }
    /// <example>1500</example>
    public decimal Precio { get; set; }
    /// <example>10</example>
    public int Stock { get; set; }
    /// <example>Electrónica</example>
    public required string Categoria { get; set; }
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime FechaCreacion { get; set; }
}