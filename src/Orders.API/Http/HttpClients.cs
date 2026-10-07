namespace Orders.API.Http;

// Registra un HttpClient por cada microservicio que Orders consume. Las URLs salen
// de la seccion "Services" de appsettings.json, asi no quedan hardcodeadas.
public static class HttpClients
{
    public const string Users = "Users";
    public const string Products = "Products";

    public static IServiceCollection AddDownstreamHttpClients(
        this IServiceCollection services, IConfiguration configuration)
    {
        // El handler necesita el HttpContext del request en curso para leer el Correlation ID.
        services.AddHttpContextAccessor();
        services.AddTransient<CorrelationIdHandler>();

        var servicios = configuration.GetSection("Services").Get<Dictionary<string, string>>() ?? [];

        foreach (var (nombre, url) in servicios)
        {
            services.AddHttpClient(nombre, c =>
            {
                c.BaseAddress = new Uri(url);
                c.Timeout = TimeSpan.FromSeconds(10);
            })
            .AddHttpMessageHandler<CorrelationIdHandler>();
        }

        return services;
    }
}
