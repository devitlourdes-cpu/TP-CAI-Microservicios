using Orders.API.Middlewares;

namespace Orders.API.Http;

// Copia el Correlation ID del request entrante a cada llamada que Orders le hace
// a Users y a Products, asi la traza no se corta al cruzar de servicio.
public class CorrelationIdHandler(IHttpContextAccessor accessor) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var correlationId = accessor.HttpContext?.Items[CorrelationIdMiddleware.CorrelationIdHeader] as string;

        if (!string.IsNullOrWhiteSpace(correlationId))
            request.Headers.TryAddWithoutValidation(CorrelationIdMiddleware.CorrelationIdHeader, correlationId);

        return base.SendAsync(request, ct);
    }
}
