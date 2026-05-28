using BiblioTech.Domain.Entities;
using BiblioTech.Domain.Interfaces;
using BiblioTech.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BiblioTech.Infrastructure.Repositories;

public class PrestamoRepository : BaseRepository<Prestamo>, IPrestamoRepository
{
    public PrestamoRepository(BiblioTechDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Prestamo>> GetActivosByEstudianteAsync(int estudianteId)
    {
        return await _dbSet
            .Where(p => p.EstudianteId == estudianteId && p.Estado == "Activo")
            .ToListAsync();
    }
}
