using BiblioTech.Domain.Entities;
using BiblioTech.Domain.Interfaces;
using BiblioTech.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BiblioTech.Infrastructure.Repositories;

public class EstudianteRepository : BaseRepository<Estudiante>, IEstudianteRepository
{
    public EstudianteRepository(BiblioTechDbContext context) : base(context)
    {
    }

    public async Task<Estudiante?> GetByCodigoODniAsync(string codigo, string dni)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.CodigoUniversitario == codigo || e.Dni == dni);
    }
}
