namespace Notifications.API.DTOs;

/// <summary>Formato de todas las respuestas de error.</summary>
public class ErrorResponse
{
    /// <example>https://tools.ietf.org/html/rfc7231#section-6.5.4</example>
    public string Type { get; set; } = "";

    /// <example>Not Found</example>
    public string Title { get; set; } = "";

    /// <example>404</example>
    public int Status { get; set; }

    /// <example>El recurso solicitado no fue encontrado.</example>
    public string Detail { get; set; } = "";

    /// <example>/api/notifications/send</example>
    public string Instance { get; set; } = "";

    /// <example>NTF-001</example>
    public string ErrorCode { get; set; } = "";

    /// <example>El usuario destinatario no fue encontrado.</example>
    public string ErrorMessage { get; set; } = "";

    /// <summary>Sirve para encontrar el request en los logs.</summary>
    /// <example>3e80c6ce-68ea-4e3b-9952-e6e1a5d19379</example>
    public string CorrelationId { get; set; } = "";
}
