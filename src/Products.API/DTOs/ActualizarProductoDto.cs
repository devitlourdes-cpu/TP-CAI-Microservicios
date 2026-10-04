using System.ComponentModel.DataAnnotations;
namespace Products.API.DTOs;

public class ActualizarProductoDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    public string Nombre { get; set; } = "";

    public string? Descripcion { get; set; }

    [Range(0.01, 999999999, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Precio { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "La categoría es obligatoria")]
    public string Categoria { get; set; } = "";
}