using BiblioTech.Application.DTOs.Usuarios;
using BiblioTech.Domain.Entities;
using BiblioTech.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BiblioTech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public UsuariosController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var usuarios = await _unitOfWork.Usuarios.GetAllAsync();
        var dtos = usuarios.Select(u => new UsuarioDto
        {
            Id = u.Id,
            Dni = u.Dni,
            Nombres = u.Nombres,
            Email = u.Email,
            PisoArea = u.PisoArea,
            RolId = u.RolId,
            Activo = u.Activo
        });

        return Ok(dtos);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UsuarioCreateDto dto)
    {
        var existe = await _unitOfWork.Usuarios.GetByEmailAsync(dto.Email);
        if (existe != null)
            return BadRequest("El usuario con ese email ya existe.");

        var usuario = new Usuario
        {
            Dni = dto.Dni,
            Nombres = dto.Nombres,
            Email = dto.Email,
            PasswordHash = dto.PasswordHash, // En un sistema real esto debe hashearse usando BCrypt o similar
            PisoArea = dto.PisoArea,
            RolId = dto.RolId,
            Activo = true
        };

        await _unitOfWork.Usuarios.AddAsync(usuario);
        await _unitOfWork.SaveChangesAsync();

        return Created(string.Empty, new { Message = "Usuario registrado exitosamente", Id = usuario.Id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var usuario = await _unitOfWork.Usuarios.GetByEmailAsync(dto.Email);

        if (usuario == null || usuario.PasswordHash != dto.Password)
        {
            return Unauthorized(new { message = "Credenciales incorrectas" });
        }

        if (!usuario.Activo)
        {
            return Unauthorized(new { message = "El usuario está inactivo" });
        }

        var res = new UsuarioDto
        {
            Id = usuario.Id,
            Dni = usuario.Dni,
            Nombres = usuario.Nombres,
            Email = usuario.Email,
            PisoArea = usuario.PisoArea,
            RolId = usuario.RolId,
            Activo = usuario.Activo
        };

        return Ok(res);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UsuarioUpdateDto dto)
    {
        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
        if (usuario == null) return NotFound();

        usuario.Dni = dto.Dni;
        usuario.Nombres = dto.Nombres;
        usuario.Email = dto.Email;
        usuario.PisoArea = dto.PisoArea;
        usuario.RolId = dto.RolId;
        usuario.Activo = dto.Activo;

        _unitOfWork.Usuarios.Update(usuario);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
        if (usuario == null) return NotFound();

        _unitOfWork.Usuarios.Delete(usuario);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}
