namespace Products.API.DTOs;
/// <summary>Formato de todas las respuestas de error.</summary>
public class ErrorResponse
{
    public string Type { get; set; } = "";

    public string Title { get; set; } = "";

    public int Status { get; set; }

    public string Detail { get; set; } = "";

    public string Instance { get; set; } = "";

    public string ErrorCode { get; set; } = "";

    public string ErrorMessage { get; set; } = "";

    /// <summary>Sirve para encontrar el request en los logs.</summary>
    public string CorrelationId { get; set; } = "";
}
