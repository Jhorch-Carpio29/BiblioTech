using BiblioTech.Domain.Entities;

namespace BiblioTech.Domain.Interfaces;

public interface IUsuarioRepository : IBaseRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email);
}
