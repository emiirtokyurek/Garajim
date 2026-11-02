using Garajim.Core.Enums;
using Garajim.Core.Validation;
using Xunit;

namespace Garajim.Tests;

public class VehicleValidatorTests
{
    [Fact]
    public void Gecerli_arac_hatasizdir()
        => Assert.True(VehicleValidator.Validate(TestVehicles.Valid()).IsValid);

    [Fact]
    public void Bos_marka_hatalidir()
    {
        var v = TestVehicles.Valid();
        v.Brand = "  ";
        Assert.False(VehicleValidator.Validate(v).IsValid);
    }

    [Fact]
    public void Gelecekteki_model_yili_hatalidir()
    {
        var v = TestVehicles.Valid();
        v.Year = DateTime.Today.Year + 2;
        Assert.False(VehicleValidator.Validate(v).IsValid);
    }

    [Fact]
    public void Lpg_sadece_benzinlide_secilebilir()
    {
        var v = TestVehicles.Valid();
        v.PrimaryFuelType = FuelType.Diesel;
        v.HasLpg = true;
        Assert.False(VehicleValidator.Validate(v).IsValid);
    }

    [Fact]
    public void Cok_kisa_plaka_hatalidir()
    {
        var v = TestVehicles.Valid("A1");
        Assert.False(VehicleValidator.Validate(v).IsValid);
    }

    [Fact]
    public void Negatif_kilometre_hatalidir()
    {
        var v = TestVehicles.Valid();
        v.CurrentOdometer = -1;
        Assert.False(VehicleValidator.Validate(v).IsValid);
    }
}
