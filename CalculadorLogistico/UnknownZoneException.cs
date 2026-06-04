namespace CalculadorLogistico;

public class UnknownZoneException : Exception
{
    public UnknownZoneException() { }

    public UnknownZoneException(string message) : base(message) { }

    public UnknownZoneException(string message, Exception innerException) : base(message, innerException) { }
}

