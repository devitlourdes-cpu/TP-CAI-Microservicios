namespace Users.API.Exceptions;

// Regla de negocio incumplida. Lleva el status porque no siempre es el mismo:
// USR-001 es 409 y USR-004 es 403.
public class BusinessRuleException(string errorCode, string message, int statusCode = 409)
    : Exception(message)
{
    public string ErrorCode { get; } = errorCode;
    public int StatusCode { get; } = statusCode;
}
