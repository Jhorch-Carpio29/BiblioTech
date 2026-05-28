namespace BiblioTech.Domain.Entities;

public class Categoria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
}
