using BiblioTech.Domain.Interfaces;
using BiblioTech.Infrastructure.Persistence;

namespace BiblioTech.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly BiblioTechDbContext _context;

    public IPrestamoRepository Prestamos { get; private set; }
    public ILibroRepository Libros { get; private set; }
    public IEstudianteRepository Estudiantes { get; private set; }
    public IUsuarioRepository Usuarios { get; private set; }
    public ICategoriaRepository Categorias { get; private set; }
    public IEscuelaProfesionalRepository EscuelasProfesionales { get; private set; }

    public UnitOfWork(BiblioTechDbContext context)
    {
        _context = context;
        Prestamos = new PrestamoRepository(context);
        Libros = new LibroRepository(context);
        Estudiantes = new EstudianteRepository(context);
        Usuarios = new UsuarioRepository(context);
        Categorias = new CategoriaRepository(context);
        EscuelasProfesionales = new EscuelaProfesionalRepository(context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
