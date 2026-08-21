using Microsoft.AspNetCore.Diagnostics;
using Users.API.Constants;
using Users.API.Exceptions;

namespace Users.API.ExceptionHandlers;

// Un handler por tipo de excepcion. Si devuelve false, ASP.NET prueba el siguiente.
// En Program.cs se registran del mas especifico al mas generico.

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

public class BusinessRuleExceptionHandler(ILogger<BusinessRuleExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not BusinessRuleException ex) return false;

        logger.LogWarning("Regla de negocio. {ErrorCode}: {ErrorMessage}", ex.ErrorCode, ex.Message);
        await ErrorResponseWriter.WriteAsync(context, ex.StatusCode, ex.ErrorCode, ex.Message, ct);
        return true;
    }
}

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
