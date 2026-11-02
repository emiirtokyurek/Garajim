using Garajim.Core.Entities;
using Garajim.Core.Enums;
using Garajim.Core.Services;
using Xunit;

namespace Garajim.Tests;

public class FuelSummaryCalculatorTests
{
    private static FuelRecord Rec(int y, int m, int d, int odo, decimal liters, decimal cost, FuelType type = FuelType.Petrol)
        => new() { Date = new DateOnly(y, m, d), Odometer = odo, Liters = liters, TotalCost = cost, FuelType = type, IsFullTank = true };

    [Fact]
    public void Toplam_ve_bu_ay_harcama_hesaplanir()
    {
        var summary = FuelSummaryCalculator.Calculate(new[]
        {
            Rec(2026, 10, 5, 1000, 40, 1000),
            Rec(2026, 11, 3, 1500, 35, 500)
        }, new DateOnly(2026, 11, 15));

        Assert.Equal(1500m, summary.TotalCost);
        Assert.Equal(500m, summary.CurrentMonthCost);
        Assert.Equal(2, summary.RecordCount);
    }

    [Fact]
    public void Her_yakit_turu_icin_ayri_tuketim_hesaplanir()
    {
        var summary = FuelSummaryCalculator.Calculate(new[]
        {
            Rec(2026, 1, 1, 1000, 40, 1200),
            Rec(2026, 1, 10, 1500, 35, 1050),
            Rec(2026, 1, 2, 1100, 30, 400, FuelType.Lpg),
            Rec(2026, 1, 12, 1600, 55, 700, FuelType.Lpg)
        }, new DateOnly(2026, 1, 20));

        Assert.Equal(2, summary.Consumption.Count);
        Assert.Equal(FuelType.Petrol, summary.Consumption[0].FuelType);
        Assert.Equal(7m, summary.Consumption[0].Stats.LitersPer100Km);
        Assert.Equal(11m, summary.Consumption[1].Stats.LitersPer100Km);
    }

    [Fact]
    public void Genel_km_maliyeti_ilk_dolum_haric_hesaplanir()
    {
        var summary = FuelSummaryCalculator.Calculate(new[]
        {
            Rec(2026, 1, 1, 1000, 40, 1200),
            Rec(2026, 1, 10, 1500, 35, 1050)
        }, new DateOnly(2026, 1, 20));

        Assert.Equal(2.1m, summary.OverallCostPerKm);
    }

    [Fact]
    public void Kayit_yoksa_bos_ozet_doner()
    {
        var summary = FuelSummaryCalculator.Calculate(Array.Empty<FuelRecord>(), new DateOnly(2026, 1, 20));

        Assert.Equal(0, summary.RecordCount);
        Assert.Null(summary.OverallCostPerKm);
        Assert.Empty(summary.Consumption);
    }
}
