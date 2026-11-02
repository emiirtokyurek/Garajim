using Garajim.Core.Entities;
using Garajim.Core.Enums;

namespace Garajim.Core.Services;

public sealed record FuelTypeStats(FuelType FuelType, FuelStats Stats);

/// <param name="OverallCostPerKm">Tüm yakıt türleri birlikte, ilk dolumdan sonraki harcama / kat edilen mesafe.</param>
/// <param name="Consumption">Her yakıt türü için ayrı tüketim (yalnızca hesaplanabilenler).</param>
public sealed record FuelSummary(
    int RecordCount,
    decimal TotalCost,
    decimal CurrentMonthCost,
    decimal? OverallCostPerKm,
    IReadOnlyList<FuelTypeStats> Consumption)
{
    public static FuelSummary Empty { get; } = new(0, 0m, 0m, null, Array.Empty<FuelTypeStats>());
}

public static class FuelSummaryCalculator
{
    public static FuelSummary Calculate(IEnumerable<FuelRecord> records, DateOnly today)
    {
        var list = records.ToList();
        if (list.Count == 0) return FuelSummary.Empty;

        var total = list.Sum(r => r.TotalCost);
        var month = list.Where(r => r.Date.Year == today.Year && r.Date.Month == today.Month).Sum(r => r.TotalCost);

        // İlk dolumun yakıtı, kayıt başlamadan önceki yolda harcanmıştır; maliyete katılmaz.
        var first = list.OrderBy(r => r.Odometer).ThenBy(r => r.Date).First();
        var span = list.Max(r => r.Odometer) - first.Odometer;
        decimal? costPerKm = span > 0 ? (total - first.TotalCost) / span : null;

        var consumption = list
            .GroupBy(r => r.FuelType)
            .Select(g => new FuelTypeStats(g.Key, FuelCalculator.Calculate(g)))
            .Where(s => s.Stats.LitersPer100Km is not null)
            .OrderBy(s => s.FuelType)
            .ToList();

        return new FuelSummary(list.Count, total, month, costPerKm, consumption);
    }
}
