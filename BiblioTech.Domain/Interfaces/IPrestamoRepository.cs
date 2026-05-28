using BiblioTech.Domain.Entities;

namespace BiblioTech.Domain.Interfaces;

public interface IPrestamoRepository : IBaseRepository<Prestamo>
{
    Task<IEnumerable<Prestamo>> GetActivosByEstudianteAsync(int estudianteId);
}
