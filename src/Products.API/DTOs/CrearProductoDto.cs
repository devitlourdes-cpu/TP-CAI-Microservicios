using System.ComponentModel.DataAnnotations;

namespace Products.API.DTOs;

/// <summary>Datos para crear un producto.</summary>
public class CrearProductoDto
{
    /// <example>Notebook Dell XPS 15</example>
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    public string Nombre { get; set; } = "";

    /// <example>Laptop 15 pulgadas, 32GB RAM</example>
    public string? Descripcion { get; set; }

    /// <example>1500</example>
    [Range(0.01, 999999999, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Precio { get; set; }

    /// <example>10</example>
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }

    /// <example>Electrónica</example>
    [Required(ErrorMessage = "La categoría es obligatoria")]
    public string Categoria { get; set; } = "";
}