using Microsoft.AspNetCore.Mvc;
using Products.API.DTOs;
using Products.API.Services;

namespace Products.API.Controllers;

[ApiController]
[Route("api/products")]
[Produces("application/json")]
[Tags("Products")]
public class ProductsController : ControllerBase
{
    private readonly IProductoService _productoService;

    public ProductsController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    /// <summary>
    /// Obtiene el listado de productos con filtros opcionales por categoría y nombre.
    /// </summary>
    /// <response code="200">Listado de productos.</response>
    /// <response code="500">Error interno (PRD-005).</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProductoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ProductoResponseDto>>> ObtenerTodos(
        [FromQuery] string? categoria,
        [FromQuery] string? nombre)
    {
        var productos = await _productoService.ObtenerTodosAsync(categoria, nombre);
        return Ok(productos);
    }

    /// <summary>
    /// Obtiene un producto por su ID.
    /// </summary>
    /// <response code="200">Producto encontrado.</response>
    /// <response code="404">El producto no existe (PRD-001).</response>
    /// <response code="500">Error interno (PRD-005).</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductoResponseDto>> ObtenerPorId(Guid id)
    {
        var producto = await _productoService.ObtenerPorIdAsync(id);
        return Ok(producto);
    }

    /// <summary>
    /// Crea un nuevo producto.
    /// </summary>
    /// <response code="201">Producto creado.</response>
    /// <response code="400">Datos inválidos (PRD-002).</response>
    /// <response code="409">Ya existe un producto con ese nombre en la categoría (PRD-003).</response>
    /// <response code="500">Error interno (PRD-005).</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductoResponseDto>> Crear([FromBody] CrearProductoDto dto)
    {
        var producto = await _productoService.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
    }

    /// <summary>
    /// Actualiza un producto existente.
    /// </summary>
    /// <response code="200">Producto actualizado.</response>
    /// <response code="400">Datos inválidos (PRD-002).</response>
    /// <response code="404">El producto no existe (PRD-001).</response>
    /// <response code="500">Error interno (PRD-005).</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductoResponseDto>> Actualizar(Guid id, [FromBody] ActualizarProductoDto dto)
    {
        var producto = await _productoService.ActualizarAsync(id, dto);
        return Ok(producto);
    }

    /// <summary>
    /// Elimina un producto por su ID.
    /// </summary>
    /// <response code="204">Producto eliminado.</response>
    /// <response code="404">El producto no existe (PRD-001).</response>
    /// <response code="409">El producto tiene órdenes activas (PRD-004).</response>
    /// <response code="500">Error interno (PRD-005).</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        await _productoService.EliminarAsync(id);
        return NoContent();
    }
}