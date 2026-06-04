namespace CalculadorLogistico;

public class UnknownRouteException : Exception
{
    public UnknownRouteException() { }

    public UnknownRouteException(string message) : base(message) { }

    public UnknownRouteException(string message, Exception innerException) : base(message, innerException) { }
}

