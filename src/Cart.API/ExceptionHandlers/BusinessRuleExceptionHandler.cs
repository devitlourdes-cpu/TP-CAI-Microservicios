
using Microsoft.AspNetCore.Diagnostics;
using Cart.API.Exceptions;

namespace Cart.API.ExceptionHandlers;

public class BusinessRuleExceptionHandler(ILogger<BusinessRuleExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not BusinessRuleException ex) return false;

        logger.LogWarning("Regla de negocio violada. {ErrorCode}: {ErrorMessage}", ex.ErrorCode, ex.Message);
        await ErrorResponseWriter.WriteAsync(context, 422, ex.ErrorCode, ex.Message, ct);
        return true;
    }
}