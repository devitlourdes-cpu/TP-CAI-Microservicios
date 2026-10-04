namespace Products.API.Models;

/// <summary>
/// Representa un producto del catálogo de e-commerce.
/// </summary>
public class Producto
{
    public Guid Id { get; set; }

    public required string Nombre { get; set; }

    public string? Descripcion { get; set; }

    public decimal Precio { get; set; }

    public int Stock { get; set; }

    public required string Categoria { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
