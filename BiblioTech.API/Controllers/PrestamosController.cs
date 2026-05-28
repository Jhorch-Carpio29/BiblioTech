using BiblioTech.Application.DTOs.Prestamos;
using BiblioTech.Application.Interfaces;
using BiblioTech.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BiblioTech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrestamosController : ControllerBase
{
    private readonly IPrestamoService _prestamoService;
    private readonly IUnitOfWork _unitOfWork;

    public PrestamosController(IPrestamoService prestamoService, IUnitOfWork unitOfWork)
    {
        _prestamoService = prestamoService;
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarPrestamo([FromBody] PrestamoCreateDto dto)
    {
        try
        {
            var result = await _prestamoService.RegistrarPrestamoAsync(dto);
            return Created(string.Empty, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/devolucion")]
    public async Task<IActionResult> RegistrarDevolucion(Guid id)
    {
        try
        {
            var result = await _prestamoService.RegistrarDevolucionAsync(id);
            if (result)
            {
                return Ok(new { message = "Devolución registrada correctamente." });
            }
            return BadRequest("No se pudo registrar la devolución.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var prestamos = await _unitOfWork.Prestamos.GetAllAsync();
        var dtos = new List<PrestamoResponseDto>();

        foreach (var p in prestamos)
        {
            var libro = await _unitOfWork.Libros.GetByIdAsync(p.LibroId);
            var estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(p.EstudianteId);

            dtos.Add(new PrestamoResponseDto
            {
                Id = p.Id,
                LibroTitulo = libro?.Titulo ?? string.Empty,
                EstudianteNombre = estudiante?.NombresCompletos ?? string.Empty,
                FechaHoraPrestamo = p.FechaHoraPrestamo,
                FechaHoraLimite = p.FechaHoraLimite,
                FechaHoraDevolucion = p.FechaHoraDevolucion,
                Estado = p.Estado
            });
        }

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var prestamo = await _unitOfWork.Prestamos.GetByIdAsync(id);
        if (prestamo == null)
        {
            return NotFound();
        }

        var libro = await _unitOfWork.Libros.GetByIdAsync(prestamo.LibroId);
        var estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(prestamo.EstudianteId);

        var dto = new PrestamoResponseDto
        {
            Id = prestamo.Id,
            LibroTitulo = libro?.Titulo ?? string.Empty,
            EstudianteNombre = estudiante?.NombresCompletos ?? string.Empty,
            FechaHoraPrestamo = prestamo.FechaHoraPrestamo,
            FechaHoraLimite = prestamo.FechaHoraLimite,
            FechaHoraDevolucion = prestamo.FechaHoraDevolucion,
            Estado = prestamo.Estado
        };

        return Ok(dto);
    }
}
