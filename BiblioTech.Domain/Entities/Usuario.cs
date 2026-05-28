namespace BiblioTech.Domain.Entities;

public class Usuario
{
    public Guid Id { get; set; }
    public string Dni { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PisoArea { get; set; } = string.Empty;
    public int RolId { get; set; }
    public bool Activo { get; set; }

    public virtual Rol Rol { get; set; } = null!;
    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}
