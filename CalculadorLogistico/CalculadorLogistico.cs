namespace CalculadorLogistico;

using System.Globalization;

public static class CalculadorLogistico
{
    public static decimal CalcularTarifaEnvio(

        decimal kilograms,
        string originCode,
        string destinationCode,
        Dictionary<(string, string), decimal> baseTariffs)
    {
        if (string.IsNullOrWhiteSpace(originCode))
            throw new ArgumentException("Origin code cannot be null or empty.", nameof(originCode));

        if (string.IsNullOrWhiteSpace(destinationCode))
            throw new ArgumentException("Destination code cannot be null or empty.", nameof(destinationCode));

        ArgumentNullException.ThrowIfNull(baseTariffs);

        var key = (originCode, destinationCode);
        if (!baseTariffs.TryGetValue(key, out var rate))
            throw new KeyNotFoundException($"Tariff rate not found for origin '{originCode}' and destination '{destinationCode}'.");

        var result = kilograms * rate;
        return Math.Round(result, 2, MidpointRounding.AwayFromZero);
    }
}

