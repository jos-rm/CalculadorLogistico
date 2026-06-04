﻿namespace CalculadorLogistico.Tests;

using System.Collections.Generic;
using Xunit;

public class UnitTest1
{


    [Fact]
    public void Test1_DirectShipment_SJO_To_MIA_Rate2_50_Returns38_75()
    {
        // Arrange
        var baseTariffs = new Dictionary<(string, string), decimal>
        {
            [("SJO", "MIA")] = 2.50m
        };

        // Act
        var result = CalculadorLogistico.CalcularTarifaEnvio(15.5m, "SJO", "MIA", baseTariffs);

        // Assert
        Assert.Equal(38.75m, result);
    }

    [Fact]
    public void Test2_UnknownCityCode_ThrowsKeyNotFoundException()
    {
        // Arrange
        var baseTariffs = new Dictionary<(string, string), decimal>();

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() =>
            CalculadorLogistico.CalcularTarifaEnvio(15.5m, "UNKNOWN", "MIA", baseTariffs)
        );
    }
}

