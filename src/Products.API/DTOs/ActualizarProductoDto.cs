using System.ComponentModel.DataAnnotations;

namespace Products.API.DTOs;

/// <summary>Datos para actualizar un producto.</summary>
public class ActualizarProductoDto
{
    /// <example>Notebook Dell XPS 15</example>
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    public string Nombre { get; set; } = "";

    /// <example>Laptop 15 pulgadas, 64GB RAM</example>
    public string? Descripcion { get; set; }

    /// <example>1750</example>
    [Range(0.01, 999999999, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Precio { get; set; }

    /// <example>8</example>
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }

    /// <example>Electrónica</example>
    [Required(ErrorMessage = "La categoría es obligatoria")]
    public string Categoria { get; set; } = "";
}