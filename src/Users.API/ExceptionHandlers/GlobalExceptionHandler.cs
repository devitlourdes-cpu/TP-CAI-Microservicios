using Microsoft.AspNetCore.Diagnostics;
using Users.API.Constants;

namespace Users.API.ExceptionHandlers;

// Atrapa cualquier error no contemplado. Va ultimo.
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        logger.LogError(exception, "Error inesperado. {ErrorCode}", ErrorCodes.USR_006);

        // El stack trace no se devuelve nunca; en Development se agrega el mensaje para poder debuggear.
        var mensaje = env.IsDevelopment()
            ? $"Error interno al procesar el usuario. Detalle: {exception.Message}"
            : "Error interno al procesar el usuario.";

        await ErrorResponseWriter.WriteAsync(context, 500, ErrorCodes.USR_006, mensaje, ct);
        return true;
    }
}
