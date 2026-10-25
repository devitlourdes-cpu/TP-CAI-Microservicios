using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cart.API.Swagger;

// Le agrega a Swagger un ejemplo distinto para cada error de cada endpoint.
public class ErrorExamplesOperationFilter : IOperationFilter
{
    private const string CorrelationId = "3e80c6ce-68ea-4e3b-9952-e6e1a5d19379";
    private const string UsuarioEjemplo = "a1b2c3d4-0000-0000-0000-111122223333";
    private const string ProductoEjemplo = "3fa85f64-5717-4562-b3fc-2c963f66afa6";

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Responses is null) return;

        var metodo = context.ApiDescription.HttpMethod ?? "";
        var ruta = "/" + (context.ApiDescription.RelativePath ?? "");
        ruta = ruta.Replace("{userId}", UsuarioEjemplo).Replace("{productId}", ProductoEjemplo);

        foreach (var respuesta in operation.Responses)
        {
            var ejemplo = ObtenerEjemplo(metodo, respuesta.Key, ruta);
            if (ejemplo is null || respuesta.Value.Content is null) continue;

            foreach (var contenido in respuesta.Value.Content.Values)
                contenido.Example = ejemplo.DeepClone();
        }
    }

    private static JsonNode? ObtenerEjemplo(string metodo, string status, string ruta) => (metodo, status) switch
    {
        ("GET" or "PUT" or "DELETE", "404") => Crear(404, "Not Found",
            "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            "El recurso solicitado no fue encontrado.", ruta,
            "CRT-001", "No hay carrito activo para este usuario."),

        ("POST", "404") => Crear(404, "Not Found",
            "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            "El recurso solicitado no fue encontrado.", ruta,
            "CRT-002", "El producto no existe."),

        ("POST" or "PUT", "400") => Crear(400, "Bad Request",
            "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            "Los datos enviados no son válidos.", ruta,
            "CRT-004", "La cantidad debe ser mayor a cero."),

        ("POST" or "PUT", "422") => Crear(422, "Unprocessable Entity",
            "https://tools.ietf.org/html/rfc4918#section-11.2",
            "No se puede procesar la solicitud.", ruta,
            "CRT-003", "Stock insuficiente. Disponible: 1, solicitado: 5."),

        (_, "500") => Crear(500, "Internal Server Error",
            "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            "Ocurrió un error inesperado al procesar la solicitud.", ruta,
            "CRT-005", "Error interno al procesar el carrito."),

        _ => null
    };

    private static JsonNode Crear(int status, string title, string type, string detail,
        string instance, string errorCode, string errorMessage) =>
        new JsonObject
        {
            ["type"] = type,
            ["title"] = title,
            ["status"] = status,
            ["detail"] = detail,
            ["instance"] = instance,
            ["errorCode"] = errorCode,
            ["errorMessage"] = errorMessage,
            ["correlationId"] = CorrelationId
        };
}
