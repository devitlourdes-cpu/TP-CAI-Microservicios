namespace Users.API.DTOs;

/// <summary>Formato de todas las respuestas de error.</summary>
public class ErrorResponse
{
    /// <example>https://tools.ietf.org/html/rfc7231#section-6.5.9</example>
    public string Type { get; set; } = "";

    /// <example>Conflict</example>
    public string Title { get; set; } = "";

    /// <example>409</example>
    public int Status { get; set; }

    /// <example>Ya existe un recurso con esos datos.</example>
    public string Detail { get; set; } = "";

    /// <example>/api/users/register</example>
    public string Instance { get; set; } = "";

    /// <example>USR-001</example>
    public string ErrorCode { get; set; } = "";

    /// <example>El email 'maria@email.com' ya está registrado.</example>
    public string ErrorMessage { get; set; } = "";

    /// <summary>Sirve para encontrar el request en los logs.</summary>
    /// <example>3e80c6ce-68ea-4e3b-9952-e6e1a5d19379</example>
    public string CorrelationId { get; set; } = "";
}
