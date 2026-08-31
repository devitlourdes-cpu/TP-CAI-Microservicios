using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Formatting.Compact;
using Users.API.Constants;
using Users.API.ExceptionHandlers;
using Users.API.Exceptions;

const string Servicio = "Users.API";

// Serilog va primero para que cualquier error de arranque quede logueado.
// Consola legible + archivo JSON.
Log.Logger = new LoggerConfiguration()
    .Enrich.WithProperty("Service", Servicio)
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] [{Service}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(new CompactJsonFormatter(), "logs/users-api-.json",
        rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddControllers();

// Por defecto ASP.NET responde los errores de validacion en un formato que no es el que
// pide el enunciado, asi que lanzamos nuestra excepcion y la traduce el handler.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errores = context.ModelState.SelectMany(e => e.Value!.Errors.Select(x => x.ErrorMessage));
        throw new ValidationException(ErrorCodes.USR_002, $"{string.Join("; ", errores)}.");
    };
});

// Del mas especifico al mas generico.
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("El servicio responde."), tags: ["live"])
    .AddCheck("ready", () => HealthCheckResult.Healthy("El servicio esta listo."), tags: ["ready"]);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Users API",
        Version = "v1",
        Description = "Microservicio de usuarios. Todos los errores incluyen errorCode y errorMessage."
    });

    // Para que Swagger muestre los comentarios del codigo.
    var xml = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(xml)) options.IncludeXmlComments(xml, includeControllerXmlComments: true);
});

var app = builder.Build();

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
        servicio = Servicio,
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
