using Garajim.Core.Entities;
using Garajim.Core.Enums;
using Garajim.Core.Validation;
using Xunit;

namespace Garajim.Tests;

public class FuelCompatibilityTests
{
    [Fact]
    public void Lpgli_benzinli_araca_benzin_ve_lpg_girilebilir()
    {
        var v = new Vehicle { PrimaryFuelType = FuelType.Petrol, HasLpg = true };

        Assert.True(FuelCompatibility.IsAllowed(v, FuelType.Petrol));
        Assert.True(FuelCompatibility.IsAllowed(v, FuelType.Lpg));
    }

    [Fact]
    public void Lpgsiz_benzinli_araca_lpg_girilemez()
    {
        var v = new Vehicle { PrimaryFuelType = FuelType.Petrol, HasLpg = false };

        Assert.False(FuelCompatibility.IsAllowed(v, FuelType.Lpg));
    }

    [Fact]
    public void Dizel_araca_yalnizca_dizel_girilebilir()
    {
        var v = new Vehicle { PrimaryFuelType = FuelType.Diesel };

        Assert.True(FuelCompatibility.IsAllowed(v, FuelType.Diesel));
        Assert.False(FuelCompatibility.IsAllowed(v, FuelType.Petrol));
    }

    [Fact]
    public void Elektrikli_araca_yalnizca_elektrik_girilebilir()
    {
        var v = new Vehicle { PrimaryFuelType = FuelType.Electric };

        Assert.True(FuelCompatibility.IsAllowed(v, FuelType.Electric));
        Assert.False(FuelCompatibility.IsAllowed(v, FuelType.Petrol));
    }
}
