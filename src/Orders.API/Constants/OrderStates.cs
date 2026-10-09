namespace Orders.API.Constants;

public static class OrderStates
{
    public const string Pendiente = "Pendiente";
    public const string Confirmada = "Confirmada";
    public const string Enviada = "Enviada";
    public const string Entregada = "Entregada";
    public const string Cancelada = "Cancelada";

    public static readonly string[] Todos =
        [Pendiente, Confirmada, Enviada, Entregada, Cancelada];

    // Products consulta estos dos estados para no dejar borrar un producto pedido.
    public static readonly string[] Activos = [Pendiente, Confirmada];
}
