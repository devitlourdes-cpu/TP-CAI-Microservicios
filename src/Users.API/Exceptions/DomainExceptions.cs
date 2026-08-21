namespace Users.API.Exceptions;

// Una excepcion por tipo de error. Todas llevan el codigo del catalogo (USR-001, etc).

public class NotFoundException(string errorCode, string message) : Exception(message)
{
    public string ErrorCode { get; } = errorCode;
}

public class ValidationException(string errorCode, string message) : Exception(message)
{
    public string ErrorCode { get; } = errorCode;
}

// Lleva el status porque no siempre es el mismo: USR-001 es 409 y USR-004 es 403.
public class BusinessRuleException(string errorCode, string message, int statusCode = 409)
    : Exception(message)
{
    public string ErrorCode { get; } = errorCode;
    public int StatusCode { get; } = statusCode;
}
