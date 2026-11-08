namespace Cart.API.DTOs;

/// <summary>El carrito completo de un usuario.</summary>
public class CarritoResponseDto
{
    /// <example>a1b2c3d4-0000-0000-0000-111122223333</example>
    public Guid UsuarioId { get; set; }
    public List<CarritoItemResponseDto> Items { get; set; } = [];
    /// <example>2024-03-10T10:50:00Z</example>
    public DateTime FechaActualizacion { get; set; }

    /// <example>3000</example>
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
