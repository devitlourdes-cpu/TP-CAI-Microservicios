using System.ComponentModel.DataAnnotations;

namespace Users.API.DTOs;

/// <summary>Credenciales para iniciar sesion.</summary>
public class LoginUserRequest
{
    /// <example>maria@email.com</example>
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
    public string Email { get; set; } = "";

    /// <example>MiPassword123!</example>
    [Required(ErrorMessage = "La password es obligatoria")]
    public string Password { get; set; } = "";
}
