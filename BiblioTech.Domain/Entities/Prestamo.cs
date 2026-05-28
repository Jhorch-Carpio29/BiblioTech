namespace BiblioTech.Domain.Entities;

public class Prestamo
{
    public Guid Id { get; set; }
    public int LibroId { get; set; }
    public int EstudianteId { get; set; }
    public Guid UsuarioId { get; set; }
    public DateTime FechaHoraPrestamo { get; set; }
    public DateTime FechaHoraLimite { get; set; }
    public DateTime? FechaHoraDevolucion { get; set; }
    public string Estado { get; set; } = string.Empty;

    public virtual Libro Libro { get; set; } = null!;
    public virtual Estudiante Estudiante { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;
}
