namespace BiblioTech.Application.DTOs.EscuelasProfesionales;

public class EscuelaProfesionalDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Facultad { get; set; } = string.Empty;
}

public class EscuelaProfesionalCreateDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Facultad { get; set; } = string.Empty;
}
