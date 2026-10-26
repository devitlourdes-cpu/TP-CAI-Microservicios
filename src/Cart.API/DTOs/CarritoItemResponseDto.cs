namespace Cart.API.DTOs;

/// <summary>Un producto dentro del carrito.</summary>
public class CarritoItemResponseDto
{
    public Guid ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
    public decimal Subtotal => PrecioUnitario * Cantidad;
}