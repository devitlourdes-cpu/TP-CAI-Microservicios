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

    public static bool Existe(string estado)
    {
        foreach (var e in Todos)
        {
            if (e.Equals(estado, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    public static bool PuedePasarA(string actual, string nuevo)
    {
        if (!Transiciones.TryGetValue(actual, out var permitidos))
            return false;

        foreach (var p in permitidos)
        {
            if (p.Equals(nuevo, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    public static string Normalizar(string estado)
    {
        // Existe(estado) ya se valido antes de llamar a este metodo, asi que
        // siempre hay una coincidencia: el fallback de abajo nunca se alcanza.
        foreach (var e in Todos)
        {
            if (e.Equals(estado, StringComparison.OrdinalIgnoreCase))
                return e;
        }
        return estado;
    }
}
