namespace BiblioTech.Application.DTOs.Prestamos;

public class PrestamoResponseDto
{
    public Guid Id { get; set; }
    public string LibroTitulo { get; set; } = string.Empty;
    public string EstudianteNombre { get; set; } = string.Empty;
    public DateTime FechaHoraPrestamo { get; set; }
    public DateTime FechaHoraLimite { get; set; }
    public DateTime? FechaHoraDevolucion { get; set; }
    public string Estado { get; set; } = string.Empty;
}
