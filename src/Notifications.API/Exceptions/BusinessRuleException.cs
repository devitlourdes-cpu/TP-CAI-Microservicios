namespace Notifications.API.Exceptions;

// Regla de negocio incumplida. Lleva el status porque no siempre es el mismo.
public class BusinessRuleException(string errorCode, string message, int statusCode = 409)
    : Exception(message)
{
    public string ErrorCode { get; } = errorCode;
    public int StatusCode { get; } = statusCode;
}
