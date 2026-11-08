namespace Orders.API.Http;

public static class HttpClients
{
    public const string Users = "Users";
    public const string Products = "Products";

    public static IServiceCollection AddDownstreamHttpClients(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<CorrelationIdHandler>();

        var servicios = configuration.GetSection("Services").Get<Dictionary<string, string>>() ?? [];

        foreach (var (nombre, url) in servicios)
        {
            void ConfigurarCliente(HttpClient c)
            {
                c.BaseAddress = new Uri(url);
                c.Timeout = TimeSpan.FromSeconds(10);
            }

            services.AddHttpClient(nombre, ConfigurarCliente)
                .AddHttpMessageHandler<CorrelationIdHandler>();
        }

        return services;
    }
}
