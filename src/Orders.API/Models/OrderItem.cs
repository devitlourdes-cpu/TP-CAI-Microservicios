namespace Orders.API.Models;

// El PrecioUnitario se copia del producto al crear la orden, para que un cambio
// de precio posterior en Products no altere ordenes ya hechas.
public class OrderItem
{
    public Guid ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
