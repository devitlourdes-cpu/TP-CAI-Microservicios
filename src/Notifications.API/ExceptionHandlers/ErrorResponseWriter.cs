using System.Text.Encodings.Web;
using System.Text.Json;
using Notifications.API.Middlewares;

namespace Notifications.API.ExceptionHandlers;

// Arma el body de error que pide el enunciado. Todos los handlers pasan por aca
// para que la respuesta salga siempre igual.
public static class ErrorResponseWriter
{
    private static readonly JsonSerializerOptions JsonOptions =
        new() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    public static async Task WriteAsync(HttpContext context, int statusCode,
        string errorCode, string errorMessage, CancellationToken ct)
    {
        var (type, title, detail) = Describe(statusCode);
        context.Response.StatusCode = statusCode;

        var body = new
        {
            type,
            title,
            status = statusCode,
            detail,
            instance = context.Request.Path.Value,
            errorCode,
            errorMessage,
            correlationId = context.Items[CorrelationIdMiddleware.CorrelationIdHeader] as string ?? ""
        };

        await context.Response.WriteAsJsonAsync(body, JsonOptions,
            contentType: "application/problem+json", cancellationToken: ct);
    }

    private static (string Type, string Title, string Detail) Describe(int statusCode) => statusCode switch
    {
        400 => ("https://tools.ietf.org/html/rfc7231#section-6.5.1", "Bad Request",
                "Los datos enviados no son válidos."),
        401 => ("https://tools.ietf.org/html/rfc7235#section-3.1", "Unauthorized",
                "Las credenciales no son válidas."),
        403 => ("https://tools.ietf.org/html/rfc7231#section-6.5.3", "Forbidden",
                "El acceso está prohibido."),
        404 => ("https://tools.ietf.org/html/rfc7231#section-6.5.4", "Not Found",
                "El recurso solicitado no fue encontrado."),
        409 => ("https://tools.ietf.org/html/rfc7231#section-6.5.9", "Conflict",
                "Ya existe un recurso con esos datos."),
        _   => ("https://tools.ietf.org/html/rfc7231#section-6.6.1", "Internal Server Error",
                "Ocurrió un error inesperado al procesar la solicitud.")
    };
}
