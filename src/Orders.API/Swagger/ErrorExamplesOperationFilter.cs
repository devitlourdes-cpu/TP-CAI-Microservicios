using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Orders.API.Swagger;

// Le agrega a Swagger un ejemplo distinto para cada error de cada endpoint.
public class ErrorExamplesOperationFilter : IOperationFilter
{
    private const string CorrelationId = "3e80c6ce-68ea-4e3b-9952-e6e1a5d19379";
    private const string OrdenEjemplo = "f1e2d3c4-0000-0000-0000-aabbccddeeff";
    private const string ProductoEjemplo = "3fa85f64-5717-4562-b3fc-2c963f66afa6";

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Responses is null) return;

        var metodo = context.ApiDescription.HttpMethod ?? "";
        var ruta = "/" + (context.ApiDescription.RelativePath ?? "");
        ruta = ruta.Replace("{id}", OrdenEjemplo).Replace("{productId}", ProductoEjemplo);

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
        ("POST", "404") => Crear(404, "Not Found",
            "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            "El recurso solicitado no fue encontrado.", ruta,
            "ORD-003", "Usuario no encontrado al crear la orden."),

        ("GET" or "PUT", "404") => Crear(404, "Not Found",
            "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            "El recurso solicitado no fue encontrado.", ruta,
            "ORD-001", "Orden no encontrada."),

        ("POST", "400") => Crear(400, "Bad Request",
            "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            "Los datos enviados no son válidos.", ruta,
            "ORD-002", "La orden debe tener al menos un item."),

        ("PUT", "400") => Crear(400, "Bad Request",
            "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            "Los datos enviados no son válidos.", ruta,
            "ORD-002", "El estado 'Pagada' no existe. Valores validos: Pendiente, Confirmada, Enviada, Entregada, Cancelada."),

        ("POST", "422") => Crear(422, "Unprocessable Entity",
            "https://tools.ietf.org/html/rfc4918#section-11.2",
            "No se puede procesar la solicitud.", ruta,
            "ORD-005", "Stock insuficiente para 'Notebook Dell XPS 15'. Disponible: 2, solicitado: 5."),

        ("PUT", "409") => Crear(409, "Conflict",
            "https://tools.ietf.org/html/rfc7231#section-6.5.9",
            "No se puede modificar el estado.", ruta,
            "ORD-006", "Una orden en estado 'Entregada' no puede pasar a 'Pendiente'."),

        (_, "500") => Crear(500, "Internal Server Error",
            "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            "Ocurrió un error inesperado al procesar la solicitud.", ruta,
            "ORD-007", "Error interno al procesar la orden."),

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
