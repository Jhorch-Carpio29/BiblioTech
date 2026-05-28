using BiblioTech.Application.DTOs.Prestamos;

namespace BiblioTech.Application.Interfaces;

public interface IPrestamoService
{
    Task<PrestamoResponseDto> RegistrarPrestamoAsync(PrestamoCreateDto dto);
    Task<bool> RegistrarDevolucionAsync(Guid prestamoId);
}
