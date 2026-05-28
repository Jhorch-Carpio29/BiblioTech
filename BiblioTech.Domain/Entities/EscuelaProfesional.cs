namespace BiblioTech.Domain.Entities;

public class EscuelaProfesional
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Facultad { get; set; } = string.Empty;

    public virtual ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();
}
