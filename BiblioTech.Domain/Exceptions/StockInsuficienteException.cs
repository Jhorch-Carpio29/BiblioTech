namespace BiblioTech.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando el stock disponible de un libro es insuficiente para realizar un préstamo.
/// Implementa la validación R41 de gestión de stock.
/// </summary>
public class StockInsuficienteException : Exception
{
    public StockInsuficienteException()
        : base("No hay stock disponible para este libro.")
    {
    }

    public StockInsuficienteException(string message)
        : base(message)
    {
    }

    public StockInsuficienteException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
