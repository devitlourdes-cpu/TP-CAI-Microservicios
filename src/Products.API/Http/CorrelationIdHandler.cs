using Products.API.Middlewares;

namespace Products.API.Http;

// Reenvía el X-Correlation-Id del request actual en cada llamada saliente.
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