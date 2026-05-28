namespace BiblioTech.Application.DTOs.Libros;

public class LibroDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public int StockDisponible { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
}

public class LibroCreateDto
{
    public string Isbn { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public int StockTotal { get; set; }
}

public class LibroUpdateDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public int StockTotal { get; set; }
    public int StockDisponible { get; set; }
}
