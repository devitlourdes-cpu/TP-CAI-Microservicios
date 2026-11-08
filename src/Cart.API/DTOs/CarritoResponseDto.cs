namespace Cart.API.DTOs;

/// <summary>El carrito completo de un usuario.</summary>
public class CarritoResponseDto
{
    public Guid UsuarioId { get; set; }
    public List<CarritoItemResponseDto> Items { get; set; } = [];
    public DateTime FechaActualizacion { get; set; }

    public decimal Total
    {
        get
        {
            decimal total = 0;
            foreach (var item in Items)
            {
                total += item.Subtotal;
            }
            return total;
        }
    }
}
