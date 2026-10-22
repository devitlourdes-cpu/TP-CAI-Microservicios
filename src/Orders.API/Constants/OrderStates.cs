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

    private static readonly Dictionary<string, string[]> Transiciones = new()
    {
        [Pendiente] = [Confirmada, Cancelada],
        [Confirmada] = [Enviada, Cancelada],
        [Enviada] = [Entregada],
        [Entregada] = [],
        [Cancelada] = []
    };

    public static bool Existe(string estado) =>
        Todos.Contains(estado, StringComparer.OrdinalIgnoreCase);

    public static bool PuedePasarA(string actual, string nuevo) =>
        Transiciones.TryGetValue(actual, out var permitidos) &&
        permitidos.Contains(nuevo, StringComparer.OrdinalIgnoreCase);

    public static string Normalizar(string estado) =>
        Todos.First(e => e.Equals(estado, StringComparison.OrdinalIgnoreCase));
}
