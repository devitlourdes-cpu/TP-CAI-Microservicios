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
}
