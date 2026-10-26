namespace Notifications.API.Models;

// Representa una notificacion registrada (el "envio" es simulado, no se manda nada real).
public class Notificacion
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;       // Email, SMS o Push (ver validacion en el Service)
    public string Estado { get; set; } = "Enviada";        // El enunciado solo pide simular el envio
    public DateTime FechaEnvio { get; set; }
}
