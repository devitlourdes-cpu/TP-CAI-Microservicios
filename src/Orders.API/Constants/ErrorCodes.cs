namespace Orders.API.Constants;

public static class ErrorCodes
{
    public const string ORD_001 = "ORD-001";   // 404 - Orden no encontrada
    public const string ORD_002 = "ORD-002";   // 400 - Los datos de la orden son invalidos
    public const string ORD_003 = "ORD-003";   // 404 - Usuario no encontrado al crear la orden
    public const string ORD_004 = "ORD-004";   // 404 - Producto no encontrado al crear la orden
    public const string ORD_005 = "ORD-005";   // 422 - Stock insuficiente
    public const string ORD_006 = "ORD-006";   // 409 - El estado no puede ser modificado
    public const string ORD_007 = "ORD-007";   // 500 - Error interno
}
