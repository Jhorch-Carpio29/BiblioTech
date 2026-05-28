using BiblioTech.Domain.Entities;
using BiblioTech.Domain.Interfaces;
using BiblioTech.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BiblioTech.Infrastructure.Repositories;

public class LibroRepository : BaseRepository<Libro>, ILibroRepository
{
    public LibroRepository(BiblioTechDbContext context) : base(context)
    {
    }

    public async Task<Libro?> GetByIsbnAsync(string isbn)
    {
        return await _dbSet.FirstOrDefaultAsync(l => l.Isbn == isbn);
    }
}
