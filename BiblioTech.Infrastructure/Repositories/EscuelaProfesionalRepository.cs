using BiblioTech.Domain.Entities;
using BiblioTech.Domain.Interfaces;
using BiblioTech.Infrastructure.Persistence;

namespace BiblioTech.Infrastructure.Repositories;

public class EscuelaProfesionalRepository : BaseRepository<EscuelaProfesional>, IEscuelaProfesionalRepository
{
    public EscuelaProfesionalRepository(BiblioTechDbContext context) : base(context)
    {
    }
}
