using BiblioTech.Application.DTOs.Categorias;
using BiblioTech.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BiblioTech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoriasController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categorias = await _unitOfWork.Categorias.GetAllAsync();
        var dtos = categorias.Select(c => new CategoriaDto
        {
            Id = c.Id,
            Nombre = c.Nombre
        });

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var c = await _unitOfWork.Categorias.GetByIdAsync(id);
        if (c == null) return NotFound();

        return Ok(new CategoriaDto { Id = c.Id, Nombre = c.Nombre });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoriaCreateDto dto)
    {
        var cat = new BiblioTech.Domain.Entities.Categoria { Nombre = dto.Nombre };

        await _unitOfWork.Categorias.AddAsync(cat);
        await _unitOfWork.SaveChangesAsync();

        return Created(string.Empty, new { Message = "Categoría registrada exitosamente", Id = cat.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CategoriaUpdateDto dto)
    {
        var cat = await _unitOfWork.Categorias.GetByIdAsync(id);
        if (cat == null) return NotFound();

        cat.Nombre = dto.Nombre;

        _unitOfWork.Categorias.Update(cat);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cat = await _unitOfWork.Categorias.GetByIdAsync(id);
        if (cat == null) return NotFound();

        _unitOfWork.Categorias.Delete(cat);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}
