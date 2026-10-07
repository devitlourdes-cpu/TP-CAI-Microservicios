using Microsoft.AspNetCore.Diagnostics;
using Orders.API.Constants;

namespace Orders.API.ExceptionHandlers;

// Atrapa cualquier error no contemplado. Va ultimo.
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        logger.LogError(exception, "Error inesperado. {ErrorCode}", ErrorCodes.ORD_007);

        // El stack trace no se devuelve nunca; en Development se agrega el mensaje para poder debuggear.
        var mensaje = env.IsDevelopment()
            ? $"Error interno al procesar la orden. Detalle: {exception.Message}"
            : "Error interno al procesar la orden.";

        await ErrorResponseWriter.WriteAsync(context, 500, ErrorCodes.ORD_007, mensaje, ct);
        return true;
    }
}
