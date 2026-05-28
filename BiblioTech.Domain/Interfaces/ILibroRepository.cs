using BiblioTech.Domain.Entities;

namespace BiblioTech.Domain.Interfaces;

public interface ILibroRepository : IBaseRepository<Libro>
{
    Task<Libro?> GetByIsbnAsync(string isbn);
}
