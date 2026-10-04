using System.ComponentModel.DataAnnotations;

namespace Users.API.DTOs;

/// <summary>Datos para registrar un usuario.</summary>
// Los mensajes no llevan punto final porque si fallan varios campos se juntan con ";".
public class RegisterUserRequest
{
    /// <example>María</example>
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    public string Nombre { get; set; } = "";

    /// <example>González</example>
    [Required(ErrorMessage = "El apellido es obligatorio")]
    [StringLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres")]
    public string Apellido { get; set; } = "";

    /// <example>maria@email.com</example>
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
    [StringLength(200, ErrorMessage = "El email no puede superar los 200 caracteres")]
    public string Email { get; set; } = "";

    /// <example>MiPassword123!</example>
    [Required(ErrorMessage = "La password es obligatoria")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La password debe tener entre 8 y 100 caracteres")]
    public string Password { get; set; } = "";
}
