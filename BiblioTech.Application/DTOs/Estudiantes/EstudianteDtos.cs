namespace BiblioTech.Application.DTOs.Estudiantes;

public class EstudianteDto
{
    public int Id { get; set; }
    public string CodigoUniversitario { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string NombresCompletos { get; set; } = string.Empty;
    public int EscuelaId { get; set; }
    public bool EsMoroso { get; set; }
}

public class EstudianteCreateDto
{
    public string CodigoUniversitario { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string NombresCompletos { get; set; } = string.Empty;
    public int EscuelaId { get; set; }
}

public class EstudianteUpdateDto
{
    public string NombresCompletos { get; set; } = string.Empty;
    public int EscuelaId { get; set; }
    public bool EsMoroso { get; set; }
}
