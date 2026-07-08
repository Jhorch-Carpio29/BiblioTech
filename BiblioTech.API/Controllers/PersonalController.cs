using BiblioTech.Domain.Entities;
using BiblioTech.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BiblioTech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonalController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public PersonalController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("profile/{id}")]
    public async Task<IActionResult> GetProfile(Guid id)
    {
        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
        if (usuario == null) return NotFound("Usuario no encontrado.");

        return Ok(new
        {
            usuario.Id,
            usuario.Dni,
            usuario.Nombres,
            usuario.Email,
            usuario.PisoArea
        });
    }

    [HttpPut("profile/{id}")]
    public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] ProfileUpdateDto dto)
    {
        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
        if (usuario == null) return NotFound("Usuario no encontrado.");

        usuario.Nombres = dto.Nombres;
        usuario.Email = dto.Email;
        usuario.PisoArea = dto.PisoArea;

        _unitOfWork.Usuarios.Update(usuario);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { message = "Perfil actualizado correctamente.", usuario = new { usuario.Nombres, usuario.Email, usuario.PisoArea } });
    }
}

public class ProfileUpdateDto
{
    public string Nombres { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PisoArea { get; set; } = string.Empty;
}
