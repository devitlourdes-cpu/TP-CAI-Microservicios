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
    /// <summary>
    /// Lista las ordenes, con filtro opcional por usuario.
    /// </summary>
    /// <param name="usuarioId">Id del usuario por el que filtrar.</param>
    /// <response code="200">Listado de ordenes.</response>
    /// <response code="500">Error interno (ORD-007).</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public ActionResult<List<OrderResponse>> GetAll([FromQuery] Guid? usuarioId)
    {
        return Ok(orderService.GetAll(usuarioId));
    }

    /// <summary>
    /// Obtiene el detalle de una orden.
    /// </summary>
    /// <param name="id">Id de la orden.</param>
    /// <response code="200">Orden encontrada.</response>
    /// <response code="404">La orden no existe (ORD-001).</response>
    /// <response code="500">Error interno (ORD-007).</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public ActionResult<OrderResponse> GetById(Guid id)
    {
        return Ok(orderService.GetById(id));
    }

    /// <summary>
    /// Crea una nueva orden validando el usuario, los productos y el stock.
    /// </summary>
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

    /// <summary>
    /// Indica si un producto tiene ordenes Pendiente o Confirmada. Lo consulta
    /// Products.API antes de borrar un producto, para su regla PRD-004.
    /// </summary>
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
