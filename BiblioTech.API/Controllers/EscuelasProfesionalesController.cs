using BiblioTech.Application.DTOs.EscuelasProfesionales;
using BiblioTech.Domain.Entities;
using BiblioTech.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BiblioTech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EscuelasProfesionalesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public EscuelasProfesionalesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var escuelas = await _unitOfWork.EscuelasProfesionales.GetAllAsync();
        var dtos = escuelas.Select(e => new EscuelaProfesionalDto
        {
            Id = e.Id,
            Nombre = e.Nombre,
            Facultad = e.Facultad
        });

        return Ok(dtos);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EscuelaProfesionalCreateDto dto)
    {
        var escuela = new EscuelaProfesional
        {
            Nombre = dto.Nombre,
            Facultad = dto.Facultad
        };

        await _unitOfWork.EscuelasProfesionales.AddAsync(escuela);
        await _unitOfWork.SaveChangesAsync();

        return Created(string.Empty, new { Message = "Escuela profesional registrada exitosamente", Id = escuela.Id });
    }
}
