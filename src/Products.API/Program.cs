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

// Logger inicial para capturar errores si falla el arranque
Log.Logger = new LoggerConfiguration()

    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configuración definitiva de Serilog
    builder.Host.UseSerilog((ctx, lc) => lc
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Service", "Products.API")
           .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Service}] [{Endpoint}] [CorrelationId: {CorrelationId}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.File(new CompactJsonFormatter(), "logs/products-api-.json", rollingInterval: RollingInterval.Day));

    // Servicios
    builder.Services.AddControllers();
    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errores = context.ModelState.SelectMany(e => e.Value!.Errors.Select(x => x.ErrorMessage));
            throw new ValidationException(ErrorCodes.PRD_002, $"{string.Join("; ", errores)}.");
        };
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Products API",
            Version = "v1",
            Description = "Microservicio de productos. Todos los errores incluyen errorCode y errorMessage."
        });

        // Para que Swagger muestre los comentarios y ejemplos del código.
        var xml = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
        if (File.Exists(xml)) options.IncludeXmlComments(xml, includeControllerXmlComments: true);
        options.OperationFilter<ErrorExamplesOperationFilter>();
    });
    builder.Services.AddScoped<IProductoService, ProductoService>();

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddTransient<CorrelationIdHandler>();
    builder.Services.AddHttpClient<OrdersClient>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["Services:Orders"] ?? "http://localhost:5003");
        client.Timeout = TimeSpan.FromSeconds(10);
    }).AddHttpMessageHandler<CorrelationIdHandler>();

    builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
    builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
    builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddHealthChecks()
        .AddCheck("self", () => HealthCheckResult.Healthy("El servicio responde."), tags: ["live"])
        .AddCheck("productos", () => HealthCheckResult.Healthy("El servicio de productos responde."), tags: ["ready"]);
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
        Predicate = c => c.Tags.Contains("live"),
        ResponseWriter = EscribirHealth
    });
    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = c => c.Tags.Contains("ready"),
        ResponseWriter = EscribirHealth
    });
   

    app.Run();
    static Task EscribirHealth(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var body = new
        {
            servicio = "Products.API",
            estado = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                nombre = e.Key,
                estado = e.Value.Status.ToString(),
                descripcion = e.Value.Description
            })
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