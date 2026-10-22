using Microsoft.AspNetCore.Mvc;
using Notifications.API.DTOs;
using Notifications.API.Services;

namespace Notifications.API.Controllers;

/// <summary>Notificaciones registradas y enviadas (simuladas) a los usuarios.</summary>
[ApiController]
[Route("api/notifications")]
[Tags("Notifications")]
[Produces("application/json")]
public class NotificationsController(INotificationService notificationService) : ControllerBase
{
    /// <summary>Registra y simula el envio de una notificacion.</summary>
    /// <remarks>
    /// El envio es simulado: no se manda ningun email/SMS real, solo se registra la
    /// notificacion con estado "Enviada".
    ///
    ///     POST /api/notifications/send
    ///     {
    ///       "usuarioId": "a1b2c3d4-0000-0000-0000-111122223333",
    ///       "mensaje": "Su orden #f1e2d3c4 fue confirmada.",
    ///       "tipo": "Email"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Notificacion registrada y "enviada".</response>
    /// <response code="400">Datos invalidos o tipo no reconocido (NTF-002).</response>
    /// <response code="404">El usuario no existe en Users.API (NTF-001).</response>
    /// <response code="500">Error interno (NTF-004).</response>
    [HttpPost("send")]
    [ProducesResponseType(typeof(NotificacionResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<NotificacionResponseDto>> Enviar(
        [FromBody] EnviarNotificacionDto dto, CancellationToken ct)
    {
        var response = await notificationService.EnviarAsync(dto, ct);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>Lista las notificaciones registradas de un usuario.</summary>
    /// <response code="200">Notificaciones encontradas.</response>
    /// <response code="404">El usuario no tiene notificaciones registradas (NTF-003).</response>
    /// <response code="500">Error interno (NTF-004).</response>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(List<NotificacionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public ActionResult<List<NotificacionResponseDto>> ObtenerPorUsuario(Guid userId)
    {
        return Ok(notificationService.ObtenerPorUsuario(userId));
    }
}