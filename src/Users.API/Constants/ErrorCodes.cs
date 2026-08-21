namespace Users.API.Constants;

// Catalogo de errores de Users (seccion 4.2 del enunciado).
public static class ErrorCodes
{
    public const string USR_001 = "USR-001";   // 409 - El email ya esta registrado
    public const string USR_002 = "USR-002";   // 400 - Los datos del usuario son invalidos
    public const string USR_003 = "USR-003";   // 401 - Credenciales incorrectas
    public const string USR_004 = "USR-004";   // 403 - Bloqueado por intentos fallidos
    public const string USR_005 = "USR-005";   // 403 - Bloqueado por fraude
    public const string USR_006 = "USR-006";   // 500 - Error interno
}
