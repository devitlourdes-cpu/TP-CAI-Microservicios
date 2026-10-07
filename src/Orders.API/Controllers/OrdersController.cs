using Microsoft.AspNetCore.Mvc;
using Orders.API.DTOs;
using Orders.API.Services;

namespace Orders.API.Controllers;

/// <summary>Gestion de ordenes de compra.</summary>
[ApiController]
[Route("api/orders")]
[Tags("Orders")]
[Produces("application/json")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    /// <summary>Crea una nueva orden.</summary>
    /// <remarks>
    /// Valida que el usuario exista en Users.API y que cada producto exista en Products.API
    /// con stock suficiente. El precio de cada item se toma del producto en ese momento
    /// y el total se calcula solo.
    ///
    ///     POST /api/orders
    ///     {
    ///       "usuarioId": "a1b2c3d4-0000-0000-0000-111122223333",
    ///       "items": [
    ///         { "productoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "cantidad": 2 }
    ///       ]
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Orden creada.</response>
    /// <response code="400">Datos invalidos o lista de items vacia (ORD-002).</response>
    /// <response code="404">El usuario (ORD-003) o algun producto (ORD-004) no existe.</response>
    /// <response code="422">Stock insuficiente (ORD-005).</response>
    /// <response code="500">Error interno (ORD-007).</response>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderResponse>> Create(
        [FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        var response = await orderService.CreateAsync(request, ct);

        return Created($"/api/orders/{response.Id}", response);
    }

    /// <summary>Indica si un producto tiene ordenes en curso.</summary>
    /// <remarks>
    /// Lo usa Products.API antes de borrar un producto: si tiene ordenes Pendiente o
    /// Confirmada, no puede eliminarse (su regla PRD-004).
    /// </remarks>
    /// <param name="productId">Id del producto a consultar.</param>
    /// <response code="200">Resultado de la consulta.</response>
    /// <response code="500">Error interno (ORD-007).</response>
    [HttpGet("product/{productId:guid}/active")]
    [ProducesResponseType(typeof(ProductActiveOrdersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public ActionResult<ProductActiveOrdersResponse> GetActiveByProduct(Guid productId)
    {
        return Ok(new ProductActiveOrdersResponse
        {
            ProductoId = productId,
            TieneOrdenesActivas = orderService.HasActiveOrders(productId)
        });
    }
}
