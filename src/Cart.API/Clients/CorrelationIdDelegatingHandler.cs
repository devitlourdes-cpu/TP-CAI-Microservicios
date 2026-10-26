using Cart.API.Middlewares;

namespace Cart.API.Clients;

// Propaga el X-Correlation-Id en las llamadas salientes hacia otros microservicios.
public class CorrelationIdDelegatingHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var correlationId = httpContextAccessor.HttpContext?.Items[CorrelationIdMiddleware.CorrelationIdHeader] as string;

        if (!string.IsNullOrWhiteSpace(correlationId))
            request.Headers.TryAddWithoutValidation(CorrelationIdMiddleware.CorrelationIdHeader, correlationId);

        return base.SendAsync(request, ct);
    }
}