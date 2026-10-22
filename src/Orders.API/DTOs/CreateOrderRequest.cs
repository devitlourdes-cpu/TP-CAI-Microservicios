using System.ComponentModel.DataAnnotations;

namespace Orders.API.DTOs;

/// <summary>Datos para crear una orden.</summary>
// Los mensajes no llevan punto final porque si fallan varios campos se juntan con ";".
public class CreateOrderRequest
{
    /// <example>a1b2c3d4-0000-0000-0000-111122223333</example>
    [Required(ErrorMessage = "El usuarioId es obligatorio")]
    public Guid UsuarioId { get; set; }

    /// <summary>Productos de la orden. No puede estar vacia.</summary>
    [Required(ErrorMessage = "La orden debe tener items")]
    [MinLength(1, ErrorMessage = "La orden debe tener al menos un item")]
    public List<CreateOrderItemRequest> Items { get; set; } = [];
}

/// <summary>Un producto dentro de la orden.</summary>
public class CreateOrderItemRequest
{
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    [Required(ErrorMessage = "El productoId es obligatorio")]
    public Guid ProductoId { get; set; }

    /// <example>2</example>
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    public int Cantidad { get; set; }
}
