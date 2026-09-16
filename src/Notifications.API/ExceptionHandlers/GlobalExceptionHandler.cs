using Microsoft.AspNetCore.Diagnostics;
using Notifications.API.Constants;

namespace Notifications.API.ExceptionHandlers;

// Atrapa cualquier error no contemplado. Va ultimo.
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        logger.LogError(exception, "Error inesperado. {ErrorCode}", ErrorCodes.NTF_004);

        var mensaje = env.IsDevelopment()
            ? $"Error interno al procesar la notificación. Detalle: {exception.Message}"
            : "Error interno al procesar la notificación.";

        await ErrorResponseWriter.WriteAsync(context, 500, ErrorCodes.NTF_004, mensaje, ct);
        return true;
    }
}
