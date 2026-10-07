namespace Users.API.DTOs;

/// <summary>Datos del usuario autenticado.</summary>
public class LoginUserResponse
{
    /// <example>a1b2c3d4-0000-0000-0000-111122223333</example>
    public Guid Id { get; set; }

    /// <example>María</example>
    public string Nombre { get; set; } = "";

    /// <example>González</example>
    public string Apellido { get; set; } = "";

    /// <example>maria@email.com</example>
    public string Email { get; set; } = "";
}
