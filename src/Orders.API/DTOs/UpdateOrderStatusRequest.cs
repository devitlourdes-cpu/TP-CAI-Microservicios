using System.ComponentModel.DataAnnotations;

namespace Orders.API.DTOs;

/// <summary>Nuevo estado de la orden.</summary>
public class UpdateOrderStatusRequest
{
    /// <example>Confirmada</example>
    [Required(ErrorMessage = "El estado es obligatorio")]
    public string Estado { get; set; } = "";
}

/// <summary>Resultado del cambio de estado.</summary>
public class OrderStatusResponse
{
    /// <example>f1e2d3c4-0000-0000-0000-aabbccddeeff</example>
    public Guid Id { get; set; }

    /// <example>Confirmada</example>
    public string Estado { get; set; } = "";

    /// <example>2024-03-10T12:00:00Z</example>
    public DateTime FechaActualizacion { get; set; }
}
