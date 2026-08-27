using Products.API.DTOs;

namespace Products.API.Services;

public interface IProductoService
{
    Task<List<ProductoResponseDto>> ObtenerTodosAsync(string? categoria, string? nombre);
    Task<ProductoResponseDto> ObtenerPorIdAsync(Guid id);
    Task<ProductoResponseDto> CrearAsync(CrearProductoDto dto);
    Task<ProductoResponseDto> ActualizarAsync(Guid id, ActualizarProductoDto dto);
    Task EliminarAsync(Guid id);
}
