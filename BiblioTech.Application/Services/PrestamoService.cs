using BiblioTech.Application.DTOs.Prestamos;
using BiblioTech.Application.Interfaces;
using BiblioTech.Domain.Entities;
using BiblioTech.Domain.Exceptions;
using BiblioTech.Domain.Interfaces;

namespace BiblioTech.Application.Services;

public class PrestamoService : IPrestamoService
{
    private readonly IUnitOfWork _unitOfWork;

    public PrestamoService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PrestamoResponseDto> RegistrarPrestamoAsync(PrestamoCreateDto dto)
    {
        var libro = await _unitOfWork.Libros.GetByIdAsync(dto.LibroId);
        if (libro == null)
            throw new InvalidOperationException("El libro no existe.");

        // R41: Validación de gestión de stock
        if (libro.StockDisponible <= 0)
            throw new StockInsuficienteException();

        var estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(dto.EstudianteId);
        if (estudiante == null)
            throw new InvalidOperationException("El estudiante no existe.");

        if (estudiante.EsMoroso)
            throw new EstudianteMorosoException();

        // Verificar automáticamente si tiene préstamos activos vencidos
        var prestamosActivos = await _unitOfWork.Prestamos.GetActivosByEstudianteAsync(estudiante.Id);
        if (prestamosActivos.Any(p => p.FechaHoraLimite < DateTime.UtcNow))
        {
            if (!estudiante.EsMoroso)
            {
                estudiante.EsMoroso = true;
                _unitOfWork.Estudiantes.Update(estudiante);
                await _unitOfWork.SaveChangesAsync();
            }
            throw new EstudianteMorosoException();
        }

        if (dto.FechaHoraLimite <= DateTime.UtcNow)
            throw new InvalidOperationException("La fecha límite debe ser mayor a la fecha actual.");

        libro.StockDisponible -= 1;
        _unitOfWork.Libros.Update(libro);

        var fechaHoraPrestamo = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        var fechaHoraLimiteUtc = DateTime.SpecifyKind(dto.FechaHoraLimite, DateTimeKind.Utc);

        var prestamo = new Prestamo
        {
            LibroId = dto.LibroId,
            EstudianteId = dto.EstudianteId,
            UsuarioId = dto.UsuarioId,
            FechaHoraPrestamo = fechaHoraPrestamo,
            FechaHoraLimite = fechaHoraLimiteUtc,
            Estado = "Activo"
        };

        await _unitOfWork.Prestamos.AddAsync(prestamo);
        await _unitOfWork.SaveChangesAsync();

        return new PrestamoResponseDto
        {
            Id = prestamo.Id,
            LibroTitulo = libro.Titulo,
            EstudianteNombre = estudiante.NombresCompletos,
            FechaHoraPrestamo = prestamo.FechaHoraPrestamo,
            FechaHoraLimite = prestamo.FechaHoraLimite,
            Estado = prestamo.Estado
        };
    }

    public async Task<bool> RegistrarDevolucionAsync(Guid prestamoId)
    {
        var prestamo = await _unitOfWork.Prestamos.GetByIdAsync(prestamoId);

        if (prestamo == null || prestamo.Estado == "Devuelto")
            throw new InvalidOperationException("El préstamo no existe o ya está devuelto.");

        var fechaActual = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        prestamo.FechaHoraDevolucion = fechaActual;
        prestamo.Estado = "Devuelto";

        // Actualizar stock de libro
        var libro = await _unitOfWork.Libros.GetByIdAsync(prestamo.LibroId);
        if (libro != null)
        {
            libro.StockDisponible += 1;
            _unitOfWork.Libros.Update(libro);
        }

        // Verificar mora
        if (fechaActual > prestamo.FechaHoraLimite)
        {
            var estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(prestamo.EstudianteId);
            if (estudiante != null)
            {
                estudiante.EsMoroso = true;
                _unitOfWork.Estudiantes.Update(estudiante);
            }
        }

        _unitOfWork.Prestamos.Update(prestamo);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
