namespace BiblioTech.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando un estudiante es moroso y no puede realizar préstamos.
/// </summary>
public class EstudianteMorosoException : Exception
{
    public EstudianteMorosoException()
        : base("El estudiante es moroso y no puede realizar préstamos.")
    {
    }

    public EstudianteMorosoException(string message)
        : base(message)
    {
    }

    public EstudianteMorosoException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
