namespace BiblioTech.Domain.Entities;

public class Estudiante
{
    public int Id { get; set; }
    public string CodigoUniversitario { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string NombresCompletos { get; set; } = string.Empty;
    public int EscuelaId { get; set; }
    public bool EsMoroso { get; set; }

    public virtual EscuelaProfesional EscuelaProfesional { get; set; } = null!;
    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}
