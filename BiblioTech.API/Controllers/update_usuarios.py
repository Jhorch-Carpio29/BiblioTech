import os

p = r'd:\UNSCH\400 I\Pruebas y Aseguramiento de Calidad de Software\LaboratorioZapata\EXAMEN 01\SISTEMA DE BIBLIOTECA\BiblioTech\BiblioTech.API\Controllers\UsuariosController.cs'
with open(p, 'r') as f: c = f.read()
if 'HttpPut' not in c:
    idx = c.rfind('}')
    new_methods = '''
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
'''
    c = c[:idx] + new_methods + '}\n'
    with open(p, 'w') as f: f.write(c)
