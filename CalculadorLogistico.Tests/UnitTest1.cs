﻿namespace CalculadorLogistico.Tests;

using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Xunit;

public class UnitTest1
{
    [Fact]
    public void LogFormat_DirectShipment_MatchesExactlyExpectedTemplate()
    {
        // Arrange
        var baseTariffs = new Dictionary<(string, string), decimal>
        {
            [("SJO", "MIA")] = 2.50m
        };

        const decimal kilograms = 15.5m;
        const string origin = "SJO";
        const string destination = "MIA";
        const decimal expectedRoundedCost = 38.75m;

    // Act
        // Timestamp is dynamic; validate only format via regex.


        string log;
        var result = CalculadorLogistico.CalcularTarifaEnvio(
            kilograms,
            origin,
            destination,
            baseTariffs,
            out log);

        var after = DateTime.Now;


        // Assert (cost)
        Assert.Equal(expectedRoundedCost, result);

        // Assert (log)
        // Validate log EXACTLY (timestamp value varies). We validate the required fixed parts and
        // that the timestamp matches dd-MM-yyyy HH:mm:ss format.
        var requiredPattern = $"^En la fecha \\d{{2}}-\\d{{2}}-\\d{{4}} \\d{{2}}:\\d{{2}}:\\d{{2}} se procesó un envío de {Regex.Escape(kilograms.ToString(CultureInfo.InvariantCulture))} kg desde {Regex.Escape(origin)} hacia {Regex.Escape(destination)}\\. Costo total calculado: {Regex.Escape(expectedRoundedCost.ToString(CultureInfo.InvariantCulture))}\\.$";
        Assert.True(Regex.IsMatch(log, requiredPattern, RegexOptions.CultureInvariant), log);




    }

    [Fact]
    public void InverseRoute_With10PercentSurcharge_CalculatesCorrectly()
    {
        // Arrange
        var baseTariffs = new Dictionary<(string, string), decimal>
        {
            [("MIA", "SJO")] = 2.00m
        };

        const decimal kilograms = 10m;
        const string origin = "SJO";
        const string destination = "MIA";

        // Act
        string log;
        var result = CalculadorLogistico.CalcularTarifaEnvio(kilograms, origin, destination, baseTariffs, out log);

        // Assert: 10 * (2.00 * 1.10) = 22.00
        Assert.Equal(22.00m, result);
    }

    [Fact]
    public void TransshipmentRoute_SumsBothLegsCorrectly()
    {
        // Arrange
        var baseTariffs = new Dictionary<(string, string), decimal>
        {
            [("A", "B")] = 1.50m,
            [("B", "C")] = 2.25m
        };

        const decimal kilograms = 4m;
        const string origin = "A";
        const string destination = "C";

        // Act
        string log;
        var result = CalculadorLogistico.CalcularTarifaEnvio(kilograms, origin, destination, baseTariffs, out log);

        // Assert: 4 * (1.50 + 2.25) = 15.00
        Assert.Equal(15.00m, result);
    }

    [Fact]
    public void UnknownZone_ThrowsUnknownZoneException()
    {
        // Arrange
        var baseTariffs = new Dictionary<(string, string), decimal>
        {
            [("SJO", "MIA")] = 2.50m
        };

        // Act & Assert
        Assert.Throws<UnknownZoneException>(() =>
        {
            string log;
            CalculadorLogistico.CalcularTarifaEnvio(15.5m, "UNKNOWN", "MIA", baseTariffs, out log);
        });
    }

    [Fact]
    public void UnknownRoute_ThrowsUnknownRouteException()
    {
        // Arrange
        // Ensure both zones exist (so we get to the routing stage), but no direct/inverse/transshipment path exists.
        // Note: the code considers ANY intermediate zone from all keys; so we must ensure there is:
        // - no direct A->C
        // - no inverse C->A
        // - no B such that A->B and B->C both exist.
        var baseTariffs = new Dictionary<(string, string), decimal>
        {
            [("A", "B")] = 1.00m,
            [("C", "D")] = 2.00m,
            [("A", "E")] = 0.75m,
            [("F", "C")] = 0.60m
        };


        // Act & Assert
        Assert.Throws<UnknownRouteException>(() =>
        {
            string log;
            CalculadorLogistico.CalcularTarifaEnvio(3m, "A", "C", baseTariffs, out log);
        });
    }
}

