using Products.API.DTOs;
using Products.API.Exceptions;
using Products.API.Models;

namespace Products.API.Services;

public class ProductoService : IProductoService
{
    // Simulación en memoria por ahora — después la reemplazamos por la librería de persistencia de la cátedra
    private static readonly List<Producto> _productos = new();

    public Task<List<ProductoResponseDto>> ObtenerTodosAsync()
    {
        var resultado = _productos.Select(MapearAResponseDto).ToList();
        return Task.FromResult(resultado);
    }

    public Task<ProductoResponseDto> ObtenerPorIdAsync(Guid id)
    {
        var producto = _productos.FirstOrDefault(p => p.Id == id);

        if (producto is null)
            throw new NotFoundException("PRD-001", "Producto no encontrado.");

        return Task.FromResult(MapearAResponseDto(producto));
    }

    public Task<ProductoResponseDto> CrearAsync(CrearProductoDto dto)
    {
        var yaExiste = _productos.Any(p =>
            p.Nombre.Equals(dto.Nombre, StringComparison.OrdinalIgnoreCase) &&
            p.Categoria.Equals(dto.Categoria, StringComparison.OrdinalIgnoreCase));

        if (yaExiste)
            throw new BusinessRuleException("PRD-003", "Ya existe un producto con ese nombre en la categoría.");

        var producto = new Producto
        {
            Id = Guid.NewGuid(),
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Precio = dto.Precio,
            Stock = dto.Stock,
            Categoria = dto.Categoria,
            FechaCreacion = DateTime.UtcNow
        };

        _productos.Add(producto);

        return Task.FromResult(MapearAResponseDto(producto));
    }

    private static ProductoResponseDto MapearAResponseDto(Producto p) => new()
    {
        Id = p.Id,
        Nombre = p.Nombre,
        Descripcion = p.Descripcion,
        Precio = p.Precio,
        Stock = p.Stock,
        Categoria = p.Categoria,
        FechaCreacion = p.FechaCreacion
    };
}