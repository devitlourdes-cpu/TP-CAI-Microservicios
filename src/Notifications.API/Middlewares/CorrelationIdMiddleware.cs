using Serilog.Context;

namespace Notifications.API.Middlewares;

// Identifica cada request con un X-Correlation-Id para poder seguirlo en los logs
// y entre microservicios. Si el llamador ya trae uno, se respeta.
public class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
{
    public const string CorrelationIdHeader = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        var valoresHeader = context.Request.Headers[CorrelationIdHeader];
        string? correlationId = valoresHeader.Count > 0 ? valoresHeader[0] : null;
        if (string.IsNullOrWhiteSpace(correlationId)) correlationId = Guid.NewGuid().ToString();

        context.Items[CorrelationIdHeader] = correlationId;

        Task EscribirHeaderCorrelationId()
        {
            context.Response.Headers[CorrelationIdHeader] = correlationId;
            return Task.CompletedTask;
        }
        context.Response.OnStarting(EscribirHeaderCorrelationId);

        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("Endpoint", $"{context.Request.Method} {context.Request.Path}"))
        {
            logger.LogInformation("Request iniciado");
            await next(context);
        }
    }
}
