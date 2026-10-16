namespace Notifications.API.Constants;

// Catalogo de errores de Notifications (seccion 4.5 del enunciado).
public static class ErrorCodes
{
    public const string NTF_001 = "NTF-001";   // 404 - Usuario no encontrado
    public const string NTF_002 = "NTF-002";   // 400 - Los datos de la notificacion son invalidos
    public const string NTF_003 = "NTF-003";   // 404 - No se encontraron notificaciones para el usuario
    public const string NTF_004 = "NTF-004";   // 500 - Error interno
}
git status