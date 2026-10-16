using Notifications.API.DTOs;

namespace Notifications.API.Services;

public interface INotificationService
{
    Task<NotificacionResponseDto> EnviarAsync(EnviarNotificacionDto dto, CancellationToken ct);
    List<NotificacionResponseDto> ObtenerPorUsuario(Guid usuarioId);
}