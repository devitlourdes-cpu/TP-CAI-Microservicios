using Microsoft.AspNetCore.Diagnostics;
using Orders.API.Exceptions;

namespace Orders.API.ExceptionHandlers;

public class ValidationExceptionHandler(ILogger<ValidationExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not ValidationException ex) return false;

        logger.LogWarning("Datos invalidos. {ErrorCode}: {ErrorMessage}", ex.ErrorCode, ex.Message);
        await ErrorResponseWriter.WriteAsync(context, 400, ex.ErrorCode, ex.Message, ct);
        return true;
    }
}
