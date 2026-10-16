namespace Orders.API.Exceptions;

// Regla de negocio incumplida. Lleva el status porque no siempre es el mismo:
// ORD-005 es 422 y ORD-006 es 409.
public class BusinessRuleException(string errorCode, string message, int statusCode = 409, string? detail = null)
    : Exception(message)
{
    public string ErrorCode { get; } = errorCode;
    public int StatusCode { get; } = statusCode;
    public string? Detail { get; } = detail;
}
