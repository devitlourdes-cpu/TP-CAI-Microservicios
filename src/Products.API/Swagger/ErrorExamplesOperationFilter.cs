using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Products.API.Swagger;

// Le agrega a Swagger un ejemplo distinto para cada error de cada endpoint.
public class ErrorExamplesOperationFilter : IOperationFilter
{
    private const string CorrelationId = "3e80c6ce-68ea-4e3b-9952-e6e1a5d19379";
    private const string IdEjemplo = "3fa85f64-5717-4562-b3fc-2c963f66afa6";

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var metodo = context.ApiDescription.HttpMethod ?? "";
        var ruta = "/" + (context.ApiDescription.RelativePath ?? "");
        ruta = ruta.Replace("{id}", IdEjemplo);

        foreach (var respuesta in operation.Responses)
        {
            var ejemplo = ObtenerEjemplo(metodo, respuesta.Key, ruta);
            if (ejemplo is null || respuesta.Value.Content is null) continue;

            foreach (var contenido in respuesta.Value.Content.Values)
                contenido.Example = ejemplo;
        }
    }

    private static IOpenApiAny? ObtenerEjemplo(string metodo, string status, string ruta) => (metodo, status) switch
    {
        ("GET" or "PUT" or "DELETE", "404") => Crear(404, "Not Found",
            "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            "El recurso solicitado no fue encontrado.", ruta,
            "PRD-001", "Producto no encontrado."),

        ("POST" or "PUT", "400") => Crear(400, "Bad Request",
            "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            "Los datos enviados no son válidos.", ruta,
            "PRD-002", "El precio debe ser mayor a 0."),

        ("POST", "409") => Crear(409, "Conflict",
            "https://tools.ietf.org/html/rfc7231#section-6.5.9",
            "Ya existe un recurso con esos datos.", ruta,
            "PRD-003", "Ya existe un producto con ese nombre en la categoría 'Electrónica'."),

        ("DELETE", "409") => Crear(409, "Conflict",
            "https://tools.ietf.org/html/rfc7231#section-6.5.9",
            "No se puede eliminar el recurso.", ruta,
            "PRD-004", "El producto tiene órdenes activas y no puede eliminarse."),

        (_, "500") => Crear(500, "Internal Server Error",
            "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            "Ocurrió un error inesperado al procesar la solicitud.", ruta,
            "PRD-005", "Error interno al procesar el producto."),

        _ => null
    };

    private static IOpenApiAny Crear(int status, string title, string type, string detail,
        string instance, string errorCode, string errorMessage) =>
        new OpenApiObject
        {
            ["type"] = new OpenApiString(type),
            ["title"] = new OpenApiString(title),
            ["status"] = new OpenApiInteger(status),
            ["detail"] = new OpenApiString(detail),
            ["instance"] = new OpenApiString(instance),
            ["errorCode"] = new OpenApiString(errorCode),
            ["errorMessage"] = new OpenApiString(errorMessage),
            ["correlationId"] = new OpenApiString(CorrelationId)
        };
}