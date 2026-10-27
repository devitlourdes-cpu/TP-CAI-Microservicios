namespace Cart.API.DTOs;

/// <summary>Un producto dentro del carrito.</summary>
public class CarritoItemResponseDto
{
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid ProductoId { get; set; }
    /// <example>Notebook Dell XPS 15</example>
    public string NombreProducto { get; set; } = string.Empty;
    /// <example>1500</example>
    public decimal PrecioUnitario { get; set; }
    /// <example>2</example>
    public int Cantidad { get; set; }
    /// <example>3000</example>
    public decimal Subtotal => PrecioUnitario * Cantidad;
}