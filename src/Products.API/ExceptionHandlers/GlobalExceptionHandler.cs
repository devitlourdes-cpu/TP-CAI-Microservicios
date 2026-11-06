using Products.API.Constants;
using Microsoft.AspNetCore.Diagnostics;

namespace Products.API.ExceptionHandlers;

// Atrapa cualquier error no contemplado. Va ultimo.
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IConfiguration config ) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        logger.LogError(exception, "Error inesperado. {ErrorCode}", ErrorCodes.PRD_005);

        var mensaje = config.GetValue<bool>("ErrorHandling:IncludeExceptionDetail")
            ? $"Error interno al procesar el producto. Detalle: {exception.Message}"
            : "Error interno al procesar el producto.";

        await ErrorResponseWriter.WriteAsync(context, 500, ErrorCodes.PRD_005, mensaje, ct);
        return true;
    }
}
