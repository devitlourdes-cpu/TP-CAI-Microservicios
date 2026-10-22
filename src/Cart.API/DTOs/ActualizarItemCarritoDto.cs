using System.ComponentModel.DataAnnotations;

namespace Cart.API.DTOs;

/// <summary>Datos para actualizar la cantidad de un item del carrito.</summary>
public class ActualizarItemCarritoDto
{
    /// <example>5</example>
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
    public int Cantidad { get; set; }
}