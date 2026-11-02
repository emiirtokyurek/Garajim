using Garajim.Core.Entities;
using Garajim.Core.Services;
using Xunit;

namespace Garajim.Tests;

public class FuelRecordCompleterTests
{
    [Fact]
    public void Eksik_toplam_litre_ve_fiyattan_hesaplanir()
    {
        var record = new FuelRecord { Liters = 40, UnitPrice = 30.5m };

        FuelRecordCompleter.Complete(record);

        Assert.Equal(1220m, record.TotalCost);
    }

    [Fact]
    public void Eksik_birim_fiyat_litre_ve_toplamdan_hesaplanir()
    {
        var record = new FuelRecord { Liters = 40, TotalCost = 1220 };

        FuelRecordCompleter.Complete(record);

        Assert.Equal(30.5m, record.UnitPrice);
    }

    [Fact]
    public void Ucu_de_doluysa_dokunulmaz()
    {
        var record = new FuelRecord { Liters = 40, UnitPrice = 30, TotalCost = 1300 };

        FuelRecordCompleter.Complete(record);

        Assert.Equal(1300m, record.TotalCost);
        Assert.Equal(30m, record.UnitPrice);
    }
}
