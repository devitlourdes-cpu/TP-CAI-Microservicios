namespace Orders.API.Constants;

// Estados posibles de una orden (Apendice A del enunciado).
public static class OrderStates
{
    public const string Pendiente = "Pendiente";
    public const string Confirmada = "Confirmada";
    public const string Enviada = "Enviada";
    public const string Entregada = "Entregada";
    public const string Cancelada = "Cancelada";

    public static readonly string[] Todos =
        [Pendiente, Confirmada, Enviada, Entregada, Cancelada];

    // Una orden activa es la que todavia esta en curso. Products las consulta para
    // no dejar borrar un producto que alguien tiene pedido (su regla PRD-004).
    public static readonly string[] Activos = [Pendiente, Confirmada];
}
