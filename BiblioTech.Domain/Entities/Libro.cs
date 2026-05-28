namespace BiblioTech.Domain.Entities;

public class Libro
{
    public int Id { get; set; }
    public string Isbn { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public int StockTotal { get; set; }
    public int StockDisponible { get; set; }

    public virtual Categoria Categoria { get; set; } = null!;
    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}
