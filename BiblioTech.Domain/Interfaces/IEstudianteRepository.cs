using BiblioTech.Domain.Entities;

namespace BiblioTech.Domain.Interfaces;

public interface IEstudianteRepository : IBaseRepository<Estudiante>
{
    Task<Estudiante?> GetByCodigoODniAsync(string codigo, string dni);
}
