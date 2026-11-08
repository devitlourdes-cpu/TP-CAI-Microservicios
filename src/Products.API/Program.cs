using Products.API.Constants;
using Products.API.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Products.API.ExceptionHandlers;
using Products.API.Exceptions;
using Products.API.Middlewares;
using Products.API.Services;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System.Reflection;
using Products.API.Swagger;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerGen;

// Logger inicial para capturar errores si falla el arranque
Log.Logger = new LoggerConfiguration()

    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configuración definitiva de Serilog
    builder.Host.UseSerilog(ConfigurarSerilog);

    // Servicios
    builder.Services.AddControllers();
    builder.Services.Configure<ApiBehaviorOptions>(ConfigurarComportamientoApi);

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(ConfigurarSwagger);
    builder.Services.AddScoped<IProductoService, ProductoService>();

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddTransient<CorrelationIdHandler>();
    builder.Services.AddHttpClient<OrdersClient>(ConfigurarClienteOrders)
        .AddHttpMessageHandler<CorrelationIdHandler>();

    builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
    builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
    builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddHealthChecks()
        .AddCheck("self", ChequeoSelf, tags: ["live"])
        .AddCheck("productos", ChequeoProductos, tags: ["ready"]);
    var app = builder.Build();

    // Pipeline
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseExceptionHandler();
    app.UseAuthorization();

    app.MapControllers();

    app.MapHealthChecks("/health", new HealthCheckOptions { ResponseWriter = EscribirHealth });
    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = EsLive,
        ResponseWriter = EscribirHealth
    });
    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = EsReady,
        ResponseWriter = EscribirHealth
    });


    app.Run();

    // --- Configuracion y helpers, como metodos con nombre en vez de lambdas ---

    static void ConfigurarSerilog(HostBuilderContext ctx, LoggerConfiguration lc)
    {
        lc.MinimumLevel.Information()
          .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
          .Enrich.FromLogContext()
          .Enrich.WithProperty("Service", "Products.API")
          .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Service}] [{Endpoint}] [CorrelationId: {CorrelationId}] {Message:lj}{NewLine}{Exception}")
          .WriteTo.File(new CompactJsonFormatter(), "logs/products-api-.json", rollingInterval: RollingInterval.Day);
    }

    static void ConfigurarComportamientoApi(ApiBehaviorOptions options)
    {
        options.InvalidModelStateResponseFactory = ManejarEstadoInvalido;
    }

    static IActionResult ManejarEstadoInvalido(ActionContext context)
    {
        var errores = new List<string>();
        foreach (var entry in context.ModelState.Values)
        {
            foreach (var error in entry.Errors)
            {
                errores.Add(error.ErrorMessage);
            }
        }
        throw new ValidationException(ErrorCodes.PRD_002, $"{string.Join("; ", errores)}.");
    }

    static void ConfigurarSwagger(SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Products API",
            Version = "v1",
            Description = "Microservicio de productos. Todos los errores incluyen errorCode y errorMessage."
        });

        var xml = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
        if (File.Exists(xml)) options.IncludeXmlComments(xml, includeControllerXmlComments: true);
        options.OperationFilter<ErrorExamplesOperationFilter>();
    }

    void ConfigurarClienteOrders(HttpClient client)
    {
        client.BaseAddress = new Uri(builder.Configuration["Services:Orders"] ?? "http://localhost:5003");
        client.Timeout = TimeSpan.FromSeconds(10);
    }

    static HealthCheckResult ChequeoSelf()
    {
        return HealthCheckResult.Healthy("El servicio responde.");
    }

    static HealthCheckResult ChequeoProductos()
    {
        return HealthCheckResult.Healthy("El servicio de productos responde.");
    }

    static bool EsLive(HealthCheckRegistration c)
    {
        return c.Tags.Contains("live");
    }

    static bool EsReady(HealthCheckRegistration c)
    {
        return c.Tags.Contains("ready");
    }

    static Task EscribirHealth(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var checks = new List<object>();
        foreach (var entry in report.Entries)
        {
            checks.Add(new
            {
                nombre = entry.Key,
                estado = entry.Value.Status.ToString(),
                descripcion = entry.Value.Description
            });
        }

        var body = new
        {
            servicio = "Products.API",
            estado = report.Status.ToString(),
            checks
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(body,
            new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación falló al iniciar.");
}
finally
{
    Log.CloseAndFlush();
}
