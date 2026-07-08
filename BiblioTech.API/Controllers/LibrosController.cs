using BiblioTech.Application.DTOs.Libros;
using BiblioTech.Domain.Entities;
using BiblioTech.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BiblioTech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LibrosController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public LibrosController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var libros = await _unitOfWork.Libros.GetAllAsync();
        var dtos = new List<LibroDto>();

        foreach (var l in libros)
        {
            var categoria = await _unitOfWork.Categorias.GetByIdAsync(l.CategoriaId);
            dtos.Add(new LibroDto
            {
                Id = l.Id,
                Titulo = l.Titulo,
                Autor = l.Autor,
                StockDisponible = l.StockDisponible,
                CategoriaNombre = categoria?.Nombre ?? string.Empty
            });
        }

        return Ok(dtos);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LibroCreateDto dto)
    {
        var existeIsbn = await _unitOfWork.Libros.GetByIsbnAsync(dto.Isbn);
        if (existeIsbn != null)
            return BadRequest("El ISBN ya está registrado.");

        var libro = new Libro
        {
            Isbn = dto.Isbn,
            Titulo = dto.Titulo,
            Autor = dto.Autor,
            CategoriaId = dto.CategoriaId,
            StockTotal = dto.StockTotal,
            StockDisponible = dto.StockTotal
        };

        await _unitOfWork.Libros.AddAsync(libro);
        await _unitOfWork.SaveChangesAsync();

        return Created(string.Empty, new { Message = "Libro registrado exitosamente", Id = libro.Id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var l = await _unitOfWork.Libros.GetByIdAsync(id);
        if (l == null) return NotFound();

        var categoria = await _unitOfWork.Categorias.GetByIdAsync(l.CategoriaId);

        return Ok(new LibroDto
        {
            Id = l.Id,
            Titulo = l.Titulo,
            Autor = l.Autor,
            StockDisponible = l.StockDisponible,
            CategoriaNombre = categoria?.Nombre ?? string.Empty
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] LibroUpdateDto dto)
    {
        var libro = await _unitOfWork.Libros.GetByIdAsync(id);
        if (libro == null) return NotFound();

        libro.Titulo = dto.Titulo;
        libro.Autor = dto.Autor;
        libro.CategoriaId = dto.CategoriaId;
        libro.StockTotal = dto.StockTotal;
        libro.StockDisponible = dto.StockDisponible;

        _unitOfWork.Libros.Update(libro);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("buscar-isbn/{isbn}")]
    public async Task<IActionResult> BuscarPorIsbn(string isbn)
    {
        var libro = await _unitOfWork.Libros.GetByIsbnAsync(isbn);
        if (libro == null)
            return NotFound(new { error = "No se encontró un libro con ese ISBN." });

        var categoria = await _unitOfWork.Categorias.GetByIdAsync(libro.CategoriaId);

        return Ok(new
        {
            id = libro.Id,
            isbn = libro.Isbn,
            titulo = libro.Titulo,
            autor = libro.Autor,
            stockDisponible = libro.StockDisponible,
            categoriaNombre = categoria?.Nombre ?? string.Empty
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var libro = await _unitOfWork.Libros.GetByIdAsync(id);
        if (libro == null) return NotFound();

        _unitOfWork.Libros.Delete(libro);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}
