namespace Products.API.Exceptions;

public class BusinessRuleException(string errorCode, string message, int statusCode = 409)
    : Exception(message)
{
    public string ErrorCode { get; } = errorCode;
    public int StatusCode { get; } = statusCode;
}
