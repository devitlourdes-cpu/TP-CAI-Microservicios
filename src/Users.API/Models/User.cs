namespace Users.API.Models;

// El PasswordHash nunca sale en una respuesta: por eso las respuestas usan DTOs.
public class User
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public DateTime FechaRegistro { get; set; }
    public bool Activo { get; set; }
    public int IntentosFallidos { get; set; }
}
