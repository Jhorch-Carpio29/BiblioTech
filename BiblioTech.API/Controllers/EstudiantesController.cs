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
        try
        {
            System.Diagnostics.Debug.WriteLine("=== INICIANDO GetAll() ===");

            var estudiantes = await _unitOfWork.Estudiantes.GetAllAsync();
            System.Diagnostics.Debug.WriteLine($"✅ Estudiantes obtenidos: {estudiantes.Count()}");

            var escuelas = await _unitOfWork.EscuelasProfesionales.GetAllAsync();
            System.Diagnostics.Debug.WriteLine($"✅ Escuelas obtenidas: {escuelas.Count()}");

            bool cambiosRealizados = false;
            var respuesta = new List<object>();

            foreach (var e in estudiantes)
            {
                var escuela = escuelas.FirstOrDefault(esc => esc.Id == e.EscuelaId);

                // Auto-verificar morosidad
                if (!e.EsMoroso)
                {
                    var prestamosActivos = await _unitOfWork.Prestamos.GetActivosByEstudianteAsync(e.Id);
                    if (prestamosActivos.Any(p => p.FechaHoraLimite < DateTime.UtcNow))
                    {
                        e.EsMoroso = true;
                        _unitOfWork.Estudiantes.Update(e);
                        cambiosRealizados = true;
                    }
                }

                respuesta.Add(new
                {
                    id = e.Id,
                    codigoUniversitario = e.CodigoUniversitario,
                    dni = e.Dni,
                    nombresCompletos = e.NombresCompletos,
                    escuelaNombre = escuela?.Nombre ?? "No asignada",
                    esMoroso = e.EsMoroso
                });
            }

            if (cambiosRealizados)
            {
                await _unitOfWork.SaveChangesAsync();
            }

            System.Diagnostics.Debug.WriteLine($"=== GetAll() Completado - Retornando {respuesta.Count} registros ===");
            return Ok(respuesta);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ ERROR EN GetAll(): {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
            return BadRequest(new { error = ex.Message, stackTrace = ex.StackTrace });
        }
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

    [HttpGet("buscar-dni/{dni}")]
    public async Task<IActionResult> BuscarPorDni(string dni)
    {
        var estudiante = await _unitOfWork.Estudiantes.GetByCodigoODniAsync("", dni);
        if (estudiante == null)
            return NotFound(new { error = "No se encontró un estudiante con ese DNI." });

        // Auto-verificar morosidad
        if (!estudiante.EsMoroso)
        {
            var prestamosActivos = await _unitOfWork.Prestamos.GetActivosByEstudianteAsync(estudiante.Id);
            if (prestamosActivos.Any(p => p.FechaHoraLimite < DateTime.UtcNow))
            {
                estudiante.EsMoroso = true;
                _unitOfWork.Estudiantes.Update(estudiante);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        var escuelas = await _unitOfWork.EscuelasProfesionales.GetAllAsync();
        var escuela = escuelas.FirstOrDefault(e => e.Id == estudiante.EscuelaId);

        return Ok(new
        {
            id = estudiante.Id,
            dni = estudiante.Dni,
            codigoUniversitario = estudiante.CodigoUniversitario,
            nombresCompletos = estudiante.NombresCompletos,
            escuelaNombre = escuela?.Nombre ?? "No asignada",
            esMoroso = estudiante.EsMoroso
        });
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
