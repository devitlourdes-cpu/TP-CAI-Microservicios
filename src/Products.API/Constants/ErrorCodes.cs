namespace Products.API.Constants;

// Catalogo de errores de Products (seccion 4.1 del enunciado).
public static class ErrorCodes
{
    public const string PRD_001 = "PRD-001";   // 404 - Producto no encontrado
    public const string PRD_002 = "PRD-002";   // 400 - Los datos del producto son invalidos
    public const string PRD_003 = "PRD-003";   // 409 - Ya existe un producto con ese nombre en la categoria
    public const string PRD_004 = "PRD-004";   // 409 - El producto tiene ordenes activas
    public const string PRD_005 = "PRD-005";   // 500 - Error interno
}