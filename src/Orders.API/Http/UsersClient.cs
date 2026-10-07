using System.Net;

namespace Orders.API.Http;

// Consulta a Users.API si un usuario existe, para validar ORD-003.
public class UsersClient(IHttpClientFactory factory, ILogger<UsersClient> logger)
{
    public async Task<bool> ExistsAsync(Guid usuarioId, CancellationToken ct)
    {
        var client = factory.CreateClient(HttpClients.Users);
        var response = await client.GetAsync($"/api/users/{usuarioId}", ct);

        if (response.StatusCode == HttpStatusCode.NotFound) return false;

        // Siempre se revisa el status antes de dar la respuesta por buena.
        // Si Users falla no inventamos que el usuario no existe: es un error nuestro.
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Users respondio {StatusCode} al consultar el usuario {UsuarioId}",
                (int)response.StatusCode, usuarioId);

            throw new HttpRequestException($"Users.API respondio {(int)response.StatusCode}.");
        }

        return true;
    }
}
