using Microsoft.AspNetCore.Mvc;
using Cart.API.DTOs;
using Cart.API.Services;

namespace Cart.API.Controllers;

/// <summary>Carrito de compras de cada usuario.</summary>
[ApiController]
[Route("api/cart")]
[Tags("Cart")]
[Produces("application/json")]
public class CartController(ICartService cartService) : ControllerBase
{
    /// <summary>Obtiene el carrito de un usuario.</summary>
    /// <response code="200">Carrito encontrado.</response>
    /// <response code="404">El usuario no tiene carrito activo (CRT-001).</response>
    /// <response code="500">Error interno (CRT-005).</response>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(CarritoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CarritoResponseDto>> Obtener(Guid userId)
    {
        return Ok(await cartService.ObtenerAsync(userId));
    }

    /// <summary>Agrega un producto al carrito.</summary>
    /// <remarks>
    /// Si el usuario todavia no tenia carrito, se crea. Si el producto ya estaba en el
    /// carrito, se reemplaza la cantidad (no se suma).
    ///
    ///     POST /api/cart/{userId}/items
    ///     {
    ///       "productoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///       "cantidad": 2
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Carrito actualizado.</response>
    /// <response code="400">Cantidad invalida (CRT-004).</response>
    /// <response code="404">El producto no existe en Products.API (CRT-002).</response>
    /// <response code="422">Stock insuficiente (CRT-003).</response>
    /// <response code="500">Error interno (CRT-005).</response>
    [HttpPost("{userId:guid}/items")]
    [ProducesResponseType(typeof(CarritoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CarritoResponseDto>> AgregarItem(
        Guid userId, [FromBody] AgregarItemCarritoDto dto, CancellationToken ct)
    {
        return Ok(await cartService.AgregarItemAsync(userId, dto, ct));
    }

    /// <summary>Actualiza la cantidad de un item del carrito.</summary>
    /// <response code="200">Carrito actualizado.</response>
    /// <response code="400">Cantidad invalida (CRT-004).</response>
    /// <response code="404">No hay carrito para ese usuario (CRT-001) o el producto no existe en Products.API (CRT-002).</response>
    /// <response code="422">Stock insuficiente (CRT-003).</response>
    /// <response code="500">Error interno (CRT-005).</response>
    [HttpPut("{userId:guid}/items/{productId:guid}")]
    [ProducesResponseType(typeof(CarritoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CarritoResponseDto>> ActualizarItem(
        Guid userId, Guid productId, [FromBody] ActualizarItemCarritoDto dto, CancellationToken ct)
    {
        return Ok(await cartService.ActualizarItemAsync(userId, productId, dto, ct));
    }

    /// <summary>Quita un producto del carrito.</summary>
    /// <response code="204">Producto quitado.</response>
    /// <response code="404">No hay carrito para ese usuario (CRT-001).</response>
    /// <response code="500">Error interno (CRT-005).</response>
    [HttpDelete("{userId:guid}/items/{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> QuitarItem(Guid userId, Guid productId)
    {
        await cartService.QuitarItemAsync(userId, productId);
        return NoContent();
    }

    /// <summary>Vacia el carrito completo de un usuario.</summary>
    /// <response code="204">Carrito vaciado.</response>
    /// <response code="404">No hay carrito para ese usuario (CRT-001).</response>
    /// <response code="500">Error interno (CRT-005).</response>
    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Vaciar(Guid userId)
    {
        await cartService.VaciarAsync(userId);
        return NoContent();
    }
}