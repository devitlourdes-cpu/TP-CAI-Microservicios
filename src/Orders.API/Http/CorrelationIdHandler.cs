using Orders.API.Middlewares;

namespace Orders.API.Http;

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
