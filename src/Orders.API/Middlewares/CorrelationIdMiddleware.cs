using Serilog.Context;

namespace Orders.API.Middlewares;

// Identifica cada request con un X-Correlation-Id para poder seguirlo en los logs
// y entre microservicios. Si el llamador ya trae uno, se respeta.
public class CorrelationIdMiddleware(RequestDelegate next)
{
    public const string CorrelationIdHeader = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(correlationId)) correlationId = Guid.NewGuid().ToString();

        context.Items[CorrelationIdHeader] = correlationId;

        // Se escribe en OnStarting porque UseExceptionHandler limpia la respuesta
        // al atrapar una excepcion y se llevaria el header puesto antes.
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdHeader] = correlationId;
            return Task.CompletedTask;
        });

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }
}
