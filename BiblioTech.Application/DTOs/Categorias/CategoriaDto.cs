namespace BiblioTech.Application.DTOs.Categorias;

public class CategoriaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class CategoriaCreateDto
{
    public string Nombre { get; set; } = string.Empty;
}

public class CategoriaUpdateDto
{
    public string Nombre { get; set; } = string.Empty;
}
