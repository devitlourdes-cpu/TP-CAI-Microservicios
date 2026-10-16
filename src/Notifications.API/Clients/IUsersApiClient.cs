using System.Text.Json.Serialization;

namespace Notifications.API.Clients;

/// <summary>
/// Datos minimos que Notifications necesita de un usuario para validar el destinatario.
/// Los JsonPropertyName son necesarios porque Users.API devuelve camelCase ("id", "nombre")
/// y no hay que depender de que ReadFromJsonAsync matchee mayusculas/minusculas por las suyas.
/// </summary>
public record UsuarioDto(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("nombre")] string Nombre);

public interface IUsersApiClient
{
    /// <summary>Devuelve null si Users.API responde 404 (el usuario no existe).</summary>
    Task<UsuarioDto?> ObtenerPorIdAsync(Guid usuarioId, CancellationToken ct);
}