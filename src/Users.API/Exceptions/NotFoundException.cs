namespace Users.API.Exceptions;

// Recurso inexistente. Responde 404.
public class NotFoundException(string errorCode, string message) : Exception(message)
{
    public string ErrorCode { get; } = errorCode;
}
