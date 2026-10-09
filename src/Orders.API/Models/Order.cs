namespace Orders.API.Models;

// Entidad Orden (Apendice A). FechaActualizacion no figura en el Apendice, pero
// la respuesta del PUT /status la devuelve, asi que hace falta guardarla.
public class Order
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public List<OrderItem> Items { get; set; } = [];
    public decimal Total { get; set; }
    public string Estado { get; set; } = "";
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
