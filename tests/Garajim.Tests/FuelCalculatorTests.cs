using Garajim.Core.Entities;
using Garajim.Core.Services;
using Xunit;

namespace Garajim.Tests;

public class FuelCalculatorTests
{
    private static FuelRecord Fill(int odo, decimal liters, decimal cost = 0, bool full = true, bool missed = false)
        => new() { Odometer = odo, Liters = liters, TotalCost = cost, IsFullTank = full, MissedPrevious = missed };

    [Fact]
    public void Iki_tam_dolum_arasinda_100km_tuketimi_hesaplanir()
    {
        var stats = FuelCalculator.Calculate(new[] { Fill(1000, 40), Fill(1500, 35) });

        Assert.Equal(500, stats.DistanceKm);
        Assert.Equal(7m, stats.LitersPer100Km);
    }

    [Fact]
    public void Kismi_dolumlar_sonraki_tam_dolumda_toplanir()
    {
        var stats = FuelCalculator.Calculate(new[]
        {
            Fill(1000, 40), Fill(1200, 20, full: false), Fill(1500, 15)
        });

        Assert.Equal(35m, stats.Liters);
        Assert.Equal(7m, stats.LitersPer100Km);
    }

    [Fact]
    public void Tek_kayit_ile_hesap_yapilamaz()
    {
        var stats = FuelCalculator.Calculate(new[] { Fill(1000, 40) });

        Assert.Null(stats.LitersPer100Km);
        Assert.Null(stats.CostPerKm);
    }

    [Fact]
    public void Kacirilan_dolum_zinciri_keser()
    {
        var stats = FuelCalculator.Calculate(new[]
        {
            Fill(1000, 40),
            Fill(1500, 35, missed: true),   // arada kayıt kaçırılmış: 1000→1500 hesaba girmez
            Fill(2000, 40)                  // 1500→2000: 500 km, 40 L
        });

        Assert.Equal(500, stats.DistanceKm);
        Assert.Equal(8m, stats.LitersPer100Km);
    }

    [Fact]
    public void Kayit_sirasi_karisik_olsa_da_kmye_gore_siralanir()
    {
        var stats = FuelCalculator.Calculate(new[] { Fill(1500, 35), Fill(1000, 40) });

        Assert.Equal(7m, stats.LitersPer100Km);
    }

    [Fact]
    public void Kilometre_basina_maliyet_hesaplanir()
    {
        var stats = FuelCalculator.Calculate(new[] { Fill(1000, 40, 1200), Fill(1500, 35, 700) });

        Assert.Equal(1.4m, stats.CostPerKm);
    }
}
