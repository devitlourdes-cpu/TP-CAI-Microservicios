using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi;
using Orders.API.Constants;
using Orders.API.ExceptionHandlers;
using Orders.API.Exceptions;
using Orders.API.Http;
using Orders.API.Middlewares;
using Orders.API.Swagger;
using Orders.API.Services;
using Serilog;
using Serilog.Formatting.Compact;
using Swashbuckle.AspNetCore.SwaggerGen;

const string Servicio = "Orders.API";

// Serilog va primero para que un error de arranque tambien quede logueado.
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", Servicio)
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] [{Service}] [{Endpoint}] [CorrelationId: {CorrelationId}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(new CompactJsonFormatter(), "logs/orders-api-.json",
        rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddControllers();

// Por defecto ASP.NET responde los errores de validacion en un formato que no es el que
// pide el enunciado, asi que lanzamos nuestra excepcion y la traduce el handler.
builder.Services.Configure<ApiBehaviorOptions>(ConfigurarComportamientoApi);

// Del mas especifico al mas generico.
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddDownstreamHttpClients(builder.Configuration);
builder.Services.AddSingleton<UsersClient>();
builder.Services.AddSingleton<ProductsClient>();

// Singleton para que las ordenes no se pierdan entre requests.
builder.Services.AddSingleton<IOrderService, OrderService>();

builder.Services.AddHealthChecks()
    .AddCheck("self", ChequeoSelf, tags: ["live"])
    .AddCheck("ordenes", ChequeoOrdenes, tags: ["ready"]);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(ConfigurarSwagger);

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
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
    throw new ValidationException(ErrorCodes.ORD_002, $"{string.Join("; ", errores)}.");
}

static void ConfigurarSwagger(SwaggerGenOptions options)
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Orders API",
        Version = "v1",
        Description = "Microservicio de ordenes. Todos los errores incluyen errorCode y errorMessage."
    });

    var xml = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(xml)) options.IncludeXmlComments(xml, includeControllerXmlComments: true);

    options.OperationFilter<ErrorExamplesOperationFilter>();
}

static HealthCheckResult ChequeoSelf()
{
    return HealthCheckResult.Healthy("El servicio responde.");
}

static HealthCheckResult ChequeoOrdenes()
{
    return HealthCheckResult.Healthy("El servicio de ordenes responde.");
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
        servicio = Servicio,
        estado = report.Status.ToString(),
        checks
    };

    return context.Response.WriteAsync(JsonSerializer.Serialize(body,
        new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
}
