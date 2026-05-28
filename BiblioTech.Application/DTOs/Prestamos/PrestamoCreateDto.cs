namespace BiblioTech.Application.DTOs.Prestamos;

public class PrestamoCreateDto
{
    public Guid UsuarioId { get; set; }
    public int EstudianteId { get; set; }
    public int LibroId { get; set; }
    public DateTime FechaHoraLimite { get; set; }
}
