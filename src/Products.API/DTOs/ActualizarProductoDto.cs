using System.ComponentModel.DataAnnotations;
namespace Products.API.DTOs;

public class ActualizarProductoDto
{
    /// <example>Notebook Dell XPS 15</example>
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    /// <example>...</example>
    public string Nombre { get; set; } = "";

    /// <example>Laptop 15 pulgadas, 64GB RAM</example>
    public string? Descripcion { get; set; }

    [Range(0.01, 999999999, ErrorMessage = "El precio debe ser mayor a 0")]
    /// <example>1750</example>
    public decimal Precio { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    /// <example>8</example>
    public int Stock { get; set; }

    [Required(ErrorMessage = "La categoría es obligatoria")]
    /// <example>Electrónica</example>
    public string Categoria { get; set; } = "";
}