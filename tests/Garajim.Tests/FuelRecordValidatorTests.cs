using Garajim.Core.Entities;
using Garajim.Core.Validation;
using Xunit;

namespace Garajim.Tests;

public class FuelRecordValidatorTests
{
    private static FuelRecord Valid() => new()
    {
        Date = new DateOnly(2026, 1, 15), Odometer = 1500,
        Liters = 35, UnitPrice = 40, TotalCost = 1400
    };

    [Fact]
    public void Gecerli_kayit_hatasizdir()
        => Assert.True(FuelRecordValidator.Validate(Valid(), previousOdometer: 1000).IsValid);

    [Fact]
    public void Km_onceki_kayittan_kucukse_hatalidir()
    {
        var result = FuelRecordValidator.Validate(Valid(), previousOdometer: 2000);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void Sifir_litre_hatalidir()
    {
        var record = Valid();
        record.Liters = 0;

        Assert.False(FuelRecordValidator.Validate(record).IsValid);
    }

    [Fact]
    public void Gelecek_tarih_hatalidir()
    {
        var record = Valid();
        record.Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        Assert.False(FuelRecordValidator.Validate(record).IsValid);
    }
}
