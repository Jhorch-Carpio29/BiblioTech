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

/// <summary>
/// DTO de respuesta enriquecido para la tabla de Padrón de Estudiantes.
/// Incluye el nombre de la escuela resuelto desde la BD para sincronizar correctamente con el frontend.
/// Sigue el contrato oficial definido en BD.md.
/// </summary>
public class EstudianteResponseDto
{
    public int Id { get; set; }
    public string CodigoUniversitario { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string NombresCompletos { get; set; } = string.Empty;
    public string EscuelaNombre { get; set; } = string.Empty;
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
