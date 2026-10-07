namespace Orders.API.DTOs;

/// <summary>Indica si un producto tiene ordenes en curso.</summary>
public class ProductActiveOrdersResponse
{
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid ProductoId { get; set; }

    /// <summary>True si hay ordenes Pendiente o Confirmada con este producto.</summary>
    /// <example>true</example>
    public bool TieneOrdenesActivas { get; set; }
}
