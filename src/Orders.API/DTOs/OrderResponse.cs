namespace Orders.API.DTOs;

/// <summary>Datos de una orden.</summary>
public class OrderResponse
{
    /// <example>f1e2d3c4-0000-0000-0000-aabbccddeeff</example>
    public Guid Id { get; set; }

    /// <example>a1b2c3d4-0000-0000-0000-111122223333</example>
    public Guid UsuarioId { get; set; }

    public List<OrderItemResponse> Items { get; set; } = [];

    /// <example>3000</example>
    public decimal Total { get; set; }

    /// <example>Pendiente</example>
    public string Estado { get; set; } = "";

    /// <example>2024-03-10T11:00:00Z</example>
    public DateTime FechaCreacion { get; set; }
}

/// <summary>Un producto dentro de la orden, con el precio que tenia al crearla.</summary>
public class OrderItemResponse
{
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid ProductoId { get; set; }

    /// <example>2</example>
    public int Cantidad { get; set; }

    /// <example>1500</example>
    public decimal PrecioUnitario { get; set; }
}
