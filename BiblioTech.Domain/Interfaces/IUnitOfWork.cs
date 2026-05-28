namespace BiblioTech.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPrestamoRepository Prestamos { get; }
    ILibroRepository Libros { get; }
    IEstudianteRepository Estudiantes { get; }
    IUsuarioRepository Usuarios { get; }
    ICategoriaRepository Categorias { get; }
    IEscuelaProfesionalRepository EscuelasProfesionales { get; }

    Task<int> SaveChangesAsync();
}
