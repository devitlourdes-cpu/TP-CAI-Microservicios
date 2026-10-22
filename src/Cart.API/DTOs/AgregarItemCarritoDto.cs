using System.ComponentModel.DataAnnotations;

namespace Cart.API.DTOs;

/// <summary>Datos para agregar un producto al carrito.</summary>
public class AgregarItemCarritoDto
{
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    [Required(ErrorMessage = "El producto es obligatorio")]
    public Guid ProductoId { get; set; }

    /// <example>2</example>
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
    public int Cantidad { get; set; }
}