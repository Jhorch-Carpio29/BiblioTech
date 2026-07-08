namespace BiblioTech.Application.DTOs.Usuarios;

public class UsuarioDto
{
    public Guid Id { get; set; }
    public string Dni { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PisoArea { get; set; } = string.Empty;
    public int RolId { get; set; }
    public bool Activo { get; set; }
}

public class UsuarioCreateDto
{
    public string Dni { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PisoArea { get; set; } = string.Empty;
    public int RolId { get; set; }
}

public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}


public class UsuarioUpdateDto
{
    public string Dni { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PisoArea { get; set; } = string.Empty;
    public int RolId { get; set; }
    public bool Activo { get; set; }
}
