using Microsoft.AspNetCore.Diagnostics;
using Orders.API.Exceptions;

namespace Orders.API.ExceptionHandlers;

public class NotFoundExceptionHandler(ILogger<NotFoundExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not NotFoundException ex) return false;

        logger.LogWarning("No encontrado. {ErrorCode}: {ErrorMessage}", ex.ErrorCode, ex.Message);
        await ErrorResponseWriter.WriteAsync(context, 404, ex.ErrorCode, ex.Message, ct);
        return true;
    }
}
