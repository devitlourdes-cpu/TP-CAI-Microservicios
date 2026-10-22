using System.ComponentModel.DataAnnotations;

namespace Notifications.API.DTOs;

/// <summary>Datos para registrar y simular el envio de una notificacion.</summary>
public class EnviarNotificacionDto
{
    /// <example>a1b2c3d4-0000-0000-0000-111122223333</example>
    [Required(ErrorMessage = "El usuario destinatario es obligatorio")]
    public Guid UsuarioId { get; set; }

    /// <example>Su orden #f1e2d3c4 fue confirmada.</example>
    [Required(ErrorMessage = "El mensaje es obligatorio")]
    [StringLength(500, ErrorMessage = "El mensaje no puede superar los 500 caracteres")]
    public string Mensaje { get; set; } = "";

    /// <example>Email</example>
    [Required(ErrorMessage = "El tipo de notificacion es obligatorio")]
    public string Tipo { get; set; } = "";
}