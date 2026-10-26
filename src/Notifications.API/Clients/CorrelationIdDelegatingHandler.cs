using Notifications.API.Middlewares;

namespace Notifications.API.Clients;

// Repite el X-Correlation-Id de la request entrante en las llamadas salientes a otros
// microservicios, para poder seguir un mismo request a traves de varios servicios en los
// logs (seccion 5.5 del enunciado: "propagarlo en llamadas HTTP salientes").
public class CorrelationIdDelegatingHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var correlationId = httpContextAccessor.HttpContext?.Items[CorrelationIdMiddleware.CorrelationIdHeader] as string;

        if (!string.IsNullOrEmpty(correlationId))
            request.Headers.TryAddWithoutValidation(CorrelationIdMiddleware.CorrelationIdHeader, correlationId);

        return base.SendAsync(request, ct);
    }
}