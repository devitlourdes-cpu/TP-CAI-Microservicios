using Cart.API.DTOs;

namespace Cart.API.Services;

public interface ICartService
{
    Task<CarritoResponseDto> ObtenerAsync(Guid userId);
    Task<CarritoResponseDto> AgregarItemAsync(Guid userId, AgregarItemCarritoDto dto, CancellationToken ct);
    Task<CarritoResponseDto> ActualizarItemAsync(Guid userId, Guid productId, ActualizarItemCarritoDto dto, CancellationToken ct);
    Task QuitarItemAsync(Guid userId, Guid productId);
    Task VaciarAsync(Guid userId);
}