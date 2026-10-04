using Microsoft.AspNetCore.Diagnostics;

namespace Products.API.ExceptionHandlers;

// Atrapa cualquier error no contemplado. Va ultimo.
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        logger.LogError(exception, "Error inesperado. {ErrorCode}", "PRD-005");

        var mensaje = env.IsDevelopment()
            ? $"Error interno al procesar el producto. Detalle: {exception.Message}"
            : "Error interno al procesar el producto.";

        await ErrorResponseWriter.WriteAsync(context, 500, "PRD-005", mensaje, ct);
        return true;
    }
}
