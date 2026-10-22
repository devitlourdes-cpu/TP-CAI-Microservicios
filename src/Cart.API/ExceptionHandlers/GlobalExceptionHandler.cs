using Microsoft.AspNetCore.Diagnostics;
using Cart.API.Constants;

namespace Cart.API.ExceptionHandlers;

// Atrapa cualquier error no contemplado, incluidas fallas al llamar a Products.API
// (por ejemplo si no esta levantado). Va ultimo.
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        logger.LogError(exception, "Error inesperado. {ErrorCode}", ErrorCodes.CRT_005);

        // El stack trace no se devuelve nunca; en Development se agrega el mensaje para poder debuggear.
        var mensaje = env.IsDevelopment()
            ? $"Error interno al procesar el carrito. Detalle: {exception.Message}"
            : "Error interno al procesar el carrito.";

        await ErrorResponseWriter.WriteAsync(context, 500, ErrorCodes.CRT_005, mensaje, ct);
        return true;
    }
}