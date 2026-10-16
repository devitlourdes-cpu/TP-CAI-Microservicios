using Notifications.API.Clients;
using Notifications.API.Constants;
using Notifications.API.DTOs;
using Notifications.API.Exceptions;
using Notifications.API.Models;

namespace Notifications.API.Services;

public class NotificationService(IUsersApiClient usersApi) : INotificationService
{
    // Tipos de notificacion soportados. El enunciado no los enumera; se definen aca
    // (seccion 4.5: NTF-002 aplica cuando el tipo "no es reconocido").
    private static readonly string[] TiposValidos = ["Email", "SMS", "Push"];

    // Notificaciones en memoria hasta que este la libreria de persistencia de la catedra
    // (seccion 1 del enunciado). Igual que Users.API y Cart.API.
    private readonly List<Notificacion> _notificaciones = [];
    private readonly Lock _candado = new();

    public async Task<NotificacionResponseDto> EnviarAsync(EnviarNotificacionDto dto, CancellationToken ct)
    {
        if (!TiposValidos.Contains(dto.Tipo))
            throw new ValidationException(ErrorCodes.NTF_002,
                $"Tipo de notificacion no reconocido: '{dto.Tipo}'. Valores validos: {string.Join(", ", TiposValidos)}.");

        // La validacion contra Users.API pega por HTTP, asi que se hace fuera del lock
        // para no bloquear el resto de las operaciones mientras esperamos la respuesta.
        _ = await usersApi.ObtenerPorIdAsync(dto.UsuarioId, ct)
            ?? throw new NotFoundException(ErrorCodes.NTF_001, "Usuario no encontrado.");

        var notificacion = new Notificacion
        {
            Id = Guid.NewGuid(),
            UsuarioId = dto.UsuarioId,
            Mensaje = dto.Mensaje,
            Tipo = dto.Tipo,
            Estado = "Enviada", // El envio es simulado: no se manda nada real
            FechaEnvio = DateTime.UtcNow
        };

        lock (_candado)
        {
            _notificaciones.Add(notificacion);
        }

        return Mapear(notificacion);
    }

    public List<NotificacionResponseDto> ObtenerPorUsuario(Guid usuarioId)
    {
        lock (_candado)
        {
            var propias = _notificaciones.Where(n => n.UsuarioId == usuarioId).ToList();

            if (propias.Count == 0)
                throw new NotFoundException(ErrorCodes.NTF_003, "No se encontraron notificaciones para el usuario.");

            return propias.Select(Mapear).ToList();
        }
    }

    private static NotificacionResponseDto Mapear(Notificacion n) => new()
    {
        Id = n.Id,
        UsuarioId = n.UsuarioId,
        Mensaje = n.Mensaje,
        Tipo = n.Tipo,
        Estado = n.Estado,
        FechaEnvio = n.FechaEnvio
    };
}