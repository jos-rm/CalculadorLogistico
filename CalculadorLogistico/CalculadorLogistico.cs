namespace CalculadorLogistico;

using System.Globalization;

public static class CalculadorLogistico
{
    public static decimal CalcularTarifaEnvio(
        decimal kilograms,
        string originCode,
        string destinationCode,
        Dictionary<(string, string), decimal> baseTariffs,
        out string log)
    {
        log = string.Empty;

        if (string.IsNullOrWhiteSpace(originCode))
            throw new ArgumentException("Origin code cannot be null or empty.", nameof(originCode));

        if (string.IsNullOrWhiteSpace(destinationCode))
            throw new ArgumentException("Destination code cannot be null or empty.", nameof(destinationCode));

        ArgumentNullException.ThrowIfNull(baseTariffs);

        var zones = new HashSet<string>(baseTariffs.Keys.SelectMany(k => new[] { k.Item1, k.Item2 }));
        if (!zones.Contains(originCode))
            throw new UnknownZoneException($"Unknown origin zone '{originCode}'.");
        if (!zones.Contains(destinationCode))
            throw new UnknownZoneException($"Unknown destination zone '{destinationCode}'.");

        var directKey = (originCode, destinationCode);
        if (baseTariffs.TryGetValue(directKey, out var directRate))
        {
            var total = kilograms * directRate;
            var rounded = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            log = BuildLog(originCode, destinationCode, kilograms, rounded);
            return rounded;
        }

        // Inverse route with 10% surcharge
        var inverseKey = (destinationCode, originCode);
        if (baseTariffs.TryGetValue(inverseKey, out var inverseRate))
        {
            var total = kilograms * (inverseRate * 1.10m);
            var rounded = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            log = BuildLog(originCode, destinationCode, kilograms, rounded);
            return rounded;
        }

        // Intermediate route: A->B and B->C
        foreach (var intermediate in zones)
        {
            if (intermediate.Equals(originCode, StringComparison.Ordinal) || intermediate.Equals(destinationCode, StringComparison.Ordinal))
                continue;

            var key1 = (originCode, intermediate);
            var key2 = (intermediate, destinationCode);

            if (!baseTariffs.TryGetValue(key1, out var rate1))
                continue;
            if (!baseTariffs.TryGetValue(key2, out var rate2))
                continue;

            var total = kilograms * (rate1 + rate2);
            var rounded = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            log = BuildLog(originCode, destinationCode, kilograms, rounded);
            return rounded;
        }

        throw new UnknownRouteException($"No route found from '{originCode}' to '{destinationCode}'.");
    }

    private static string BuildLog(string origin, string destination, decimal kilograms, decimal roundedCost)
    {
        // Expected format: 
        // 'En la fecha dd-mm-yyyy hh24:mi:ss se procesó un envío de xxx kg desde <origin> hacia <destination>. Costo total calculado: yyy.'
        var now = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        return $"En la fecha {now} se procesó un envío de {kilograms} kg desde {origin} hacia {destination}. Costo total calculado: {roundedCost}.";
    }
}

