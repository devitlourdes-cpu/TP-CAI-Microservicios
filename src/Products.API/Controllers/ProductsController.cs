using Microsoft.AspNetCore.Mvc;
using Products.API.DTOs;
using Products.API.Services;

namespace Products.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductoService _productoService;

    public ProductsController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductoResponseDto>>> ObtenerTodos()
    {
        var productos = await _productoService.ObtenerTodosAsync();
        return Ok(productos);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductoResponseDto>> ObtenerPorId(Guid id)
    {
        var producto = await _productoService.ObtenerPorIdAsync(id);
        return Ok(producto);
    }

    [HttpPost]
    public async Task<ActionResult<ProductoResponseDto>> Crear([FromBody] CrearProductoDto dto)
    {
        var producto = await _productoService.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
    }
}