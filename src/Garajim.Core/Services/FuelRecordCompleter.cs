using Garajim.Core.Entities;

namespace Garajim.Core.Services;

/// <summary>Fişte genelde iki bilgi bellidir (litre + tutar ya da litre + birim fiyat); eksik üçüncüsünü hesaplar.</summary>
public static class FuelRecordCompleter
{
    public static void Complete(FuelRecord record)
    {
        if (record.Liters <= 0) return;

        if (record.TotalCost <= 0 && record.UnitPrice > 0)
            record.TotalCost = Math.Round(record.Liters * record.UnitPrice, 2, MidpointRounding.AwayFromZero);
        else if (record.UnitPrice <= 0 && record.TotalCost > 0)
            record.UnitPrice = Math.Round(record.TotalCost / record.Liters, 3, MidpointRounding.AwayFromZero);
    }
}
