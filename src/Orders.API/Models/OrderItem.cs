namespace Orders.API.Models;

/// <summary>
/// Producto dentro de una orden. El precio se copia del producto al crear la orden.
/// </summary>
public class OrderItem
{
    public Guid ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
