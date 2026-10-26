namespace Cart.API.Models;

public class Carrito
{
    public Guid UsuarioId { get; set; }
    public List<CarritoItem> Items { get; set; } = [];
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
}