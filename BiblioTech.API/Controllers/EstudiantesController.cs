using BiblioTech.Application.DTOs.Estudiantes;
using BiblioTech.Domain.Entities;
using BiblioTech.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BiblioTech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstudiantesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public EstudiantesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var estudiantes = await _unitOfWork.Estudiantes.GetAllAsync();
        var dtos = estudiantes.Select(e => new EstudianteDto
        {
            Id = e.Id,
            CodigoUniversitario = e.CodigoUniversitario,
            Dni = e.Dni,
            NombresCompletos = e.NombresCompletos,
            EscuelaId = e.EscuelaId,
            EsMoroso = e.EsMoroso
        });

        return Ok(dtos);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EstudianteCreateDto dto)
    {
        var existe = await _unitOfWork.Estudiantes.GetByCodigoODniAsync(dto.CodigoUniversitario, dto.Dni);
        if (existe != null)
            return BadRequest("El estudiante ya está registrado con ese código o DNI.");

        var estudiante = new Estudiante
        {
            CodigoUniversitario = dto.CodigoUniversitario,
            Dni = dto.Dni,
            NombresCompletos = dto.NombresCompletos,
            EscuelaId = dto.EscuelaId,
            EsMoroso = false
        };

        await _unitOfWork.Estudiantes.AddAsync(estudiante);
        await _unitOfWork.SaveChangesAsync();

        return Created(string.Empty, new { Message = "Estudiante registrado exitosamente", Id = estudiante.Id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var e = await _unitOfWork.Estudiantes.GetByIdAsync(id);
        if (e == null) return NotFound();

        return Ok(new EstudianteDto
        {
            Id = e.Id,
            CodigoUniversitario = e.CodigoUniversitario,
            Dni = e.Dni,
            NombresCompletos = e.NombresCompletos,
            EscuelaId = e.EscuelaId,
            EsMoroso = e.EsMoroso
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] EstudianteUpdateDto dto)
    {
        var estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(id);
        if (estudiante == null) return NotFound();

        estudiante.NombresCompletos = dto.NombresCompletos;
        estudiante.EscuelaId = dto.EscuelaId;
        estudiante.EsMoroso = dto.EsMoroso;

        _unitOfWork.Estudiantes.Update(estudiante);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(id);
        if (estudiante == null) return NotFound();

        _unitOfWork.Estudiantes.Delete(estudiante);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}
