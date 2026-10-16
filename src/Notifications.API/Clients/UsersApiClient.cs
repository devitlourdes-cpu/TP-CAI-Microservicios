using System.Net;
using System.Net.Http.Json;

namespace Notifications.API.Clients;

// Llama a Users.API via IHttpClientFactory (seccion 7 del enunciado).
// El BaseAddress y el header de correlacion se configuran al registrar el HttpClient en Program.cs.
public class UsersApiClient(HttpClient http) : IUsersApiClient
{
    public async Task<UsuarioDto?> ObtenerPorIdAsync(Guid usuarioId, CancellationToken ct)
    {
        var response = await http.GetAsync($"/api/users/{usuarioId}", ct);

        if (response.StatusCode == HttpStatusCode.NotFound) return null;

        // Cualquier otro codigo no exitoso (Users.API caido, 500, etc.) cae en el
        // GlobalExceptionHandler de Notifications y sale como NTF-004: no hace falta mapearlo aca.
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UsuarioDto>(cancellationToken: ct);
    }
}