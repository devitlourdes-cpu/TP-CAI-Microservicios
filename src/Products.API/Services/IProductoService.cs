using Products.API.DTOs;

namespace Products.API.Services;

public interface IProductoService
{
    Task<List<ProductoResponseDto>> ObtenerTodosAsync();
    Task<ProductoResponseDto> ObtenerPorIdAsync(Guid id);
    Task<ProductoResponseDto> CrearAsync(CrearProductoDto dto);
}
