using Products.API.ExceptionHandlers;
using Products.API.Middlewares;
using Products.API.Services;
using Serilog;
using Serilog.Formatting.Compact;

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
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Service", "Products.API")
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Service}] [CorrelationId: {CorrelationId}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.File(new CompactJsonFormatter(), "logs/products-api-.json", rollingInterval: RollingInterval.Day));

    // Servicios
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddScoped<IProductoService, ProductoService>();

    builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
    builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
    builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddHealthChecks();

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

    app.MapHealthChecks("/health");
    app.MapHealthChecks("/health/ready");
    app.MapHealthChecks("/health/live");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación falló al iniciar.");
}
finally
{
    Log.CloseAndFlush();
}