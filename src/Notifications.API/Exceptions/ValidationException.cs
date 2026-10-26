namespace Notifications.API.Exceptions;

// Datos de entrada invalidos. Responde 400.
public class ValidationException(string errorCode, string message) : Exception(message)
{
    public string ErrorCode { get; } = errorCode;
}