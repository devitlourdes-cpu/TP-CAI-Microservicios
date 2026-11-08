using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Formatting.Compact;
using Cart.API.Clients;
using Cart.API.Constants;
using Cart.API.ExceptionHandlers;
using Cart.API.Middlewares;
using Cart.API.Exceptions;
using Cart.API.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

const string Servicio = "Cart.API";

// Serilog va primero para que cualquier error de arranque quede logueado.
// Consola legible + archivo JSON.
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", Servicio)
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] [{Service}] [CorrelationId: {CorrelationId}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(new CompactJsonFormatter(), "logs/cart-api-.json",
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

// Singleton para que el contenido de los carritos no se pierda entre requests.
builder.Services.AddSingleton<ICartService, CartService>();

// Cliente HTTP hacia Products.API. Apuntamos directo a HTTPS para evitar el redirect 307
// que ya nos complico una vez con Users.API.
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<CorrelationIdDelegatingHandler>();
builder.Services.AddHttpClient<IProductsApiClient, ProductsApiClient>(ConfigurarClienteProducts)
    .AddHttpMessageHandler<CorrelationIdDelegatingHandler>();

builder.Services.AddHealthChecks()
    .AddCheck("self", ChequeoSelf, tags: ["live"])
    .AddCheck("carrito", ChequeoCarrito, tags: ["ready"]);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(ConfigurarSwagger);

var app = builder.Build();

// Va primero para que el Correlation ID exista en todos los logs del request.
app.UseMiddleware<CorrelationIdMiddleware>();

// Loguea cada request con su duracion.
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
    throw new ValidationException(ErrorCodes.CRT_004, $"{string.Join("; ", errores)}.");
}

void ConfigurarClienteProducts(HttpClient client)
{
    var baseUrl = builder.Configuration["ProductsApi:BaseUrl"] ?? "https://localhost:65084";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(5);
}

static void ConfigurarSwagger(SwaggerGenOptions options)
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Cart API",
        Version = "v1",
        Description = "Microservicio de carrito de compras. Todos los errores incluyen errorCode y errorMessage. " +
                      "Depende de Products.API para validar que el producto exista y tenga stock."
    });

    var xml = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(xml)) options.IncludeXmlComments(xml, includeControllerXmlComments: true);
}

static HealthCheckResult ChequeoSelf()
{
    return HealthCheckResult.Healthy("El servicio responde.");
}

static HealthCheckResult ChequeoCarrito()
{
    return HealthCheckResult.Healthy("El servicio de carrito responde.");
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
