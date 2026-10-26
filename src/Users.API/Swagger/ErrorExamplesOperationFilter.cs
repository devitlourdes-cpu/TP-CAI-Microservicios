using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Users.API.Swagger;

// Le agrega a Swagger un ejemplo distinto para cada error de cada endpoint.
public class ErrorExamplesOperationFilter : IOperationFilter
{
    private const string CorrelationId = "3e80c6ce-68ea-4e3b-9952-e6e1a5d19379";
    private const string UsuarioEjemplo = "a1b2c3d4-0000-0000-0000-111122223333";

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Responses is null) return;

        var metodo = context.ApiDescription.HttpMethod ?? "";
        var ruta = "/" + (context.ApiDescription.RelativePath ?? "");
        ruta = ruta.Replace("{id}", UsuarioEjemplo);

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
        ("GET", "404") => Crear(404, "Not Found",
            "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            "El recurso solicitado no fue encontrado.", ruta,
            "USR-007", "Usuario no encontrado."),

        ("POST", "400") when ruta.EndsWith("/login") => Crear(400, "Bad Request",
            "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            "Los datos enviados no son válidos.", ruta,
            "USR-002", "El email es obligatorio."),

        ("POST", "400") => Crear(400, "Bad Request",
            "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            "Los datos enviados no son válidos.", ruta,
            "USR-002", "El email no tiene un formato válido; La password debe tener entre 8 y 100 caracteres."),

        ("POST", "409") => Crear(409, "Conflict",
            "https://tools.ietf.org/html/rfc7231#section-6.5.9",
            "Ya existe un recurso con esos datos.", ruta,
            "USR-001", "El email 'maria@email.com' ya está registrado."),

        ("POST", "401") => Crear(401, "Unauthorized",
            "https://tools.ietf.org/html/rfc7235#section-3.1",
            "Las credenciales no son válidas.", ruta,
            "USR-003", "Credenciales incorrectas."),

        ("POST", "403") => Crear(403, "Forbidden",
            "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            "El acceso está prohibido.", ruta,
            "USR-004", "Su cuenta fue bloqueada por superar el máximo de intentos fallidos. Contacte a soporte."),

        (_, "500") => Crear(500, "Internal Server Error",
            "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            "Ocurrió un error inesperado al procesar la solicitud.", ruta,
            "USR-006", "Error interno al procesar el usuario."),

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
