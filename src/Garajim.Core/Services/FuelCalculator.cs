using Garajim.Core.Entities;

namespace Garajim.Core.Services;

/// <summary>Hesaplanan yakıt istatistikleri. Mesafe 0 ise oranlar null döner.</summary>
public sealed record FuelStats(int DistanceKm, decimal Liters, decimal Cost)
{
    public decimal? LitersPer100Km => DistanceKm > 0 ? Liters / DistanceKm * 100m : null;
    public decimal? CostPerKm => DistanceKm > 0 ? Cost / DistanceKm : null;
}

/// <summary>
/// "Depo dolduruldu" yöntemiyle tüketim hesabı: iki tam dolum arasındaki
/// litre toplamı / aradaki mesafe. Verilen kayıtların TEK yakıt türüne ait olması beklenir.
/// </summary>
public static class FuelCalculator
{
    public static FuelStats Calculate(IEnumerable<FuelRecord> records)
    {
        int totalDistance = 0;
        decimal totalLiters = 0, totalCost = 0;

        int? anchorOdometer = null;   // son geçerli tam dolumun km'si
        decimal litersSince = 0, costSince = 0;

        foreach (var r in records.OrderBy(x => x.Odometer).ThenBy(x => x.Date))
        {
            if (r.MissedPrevious)
            {
                // Zincir koptu: bu noktadan önceki veriye güvenemeyiz.
                anchorOdometer = r.IsFullTank ? r.Odometer : null;
                litersSince = 0;
                costSince = 0;
                continue;
            }

            if (anchorOdometer is null)
            {
                // İlk tam dolum sadece başlangıç noktasıdır; litresi öncesindeki yolun yakıtıdır.
                if (r.IsFullTank) anchorOdometer = r.Odometer;
                continue;
            }

            litersSince += r.Liters;
            costSince += r.TotalCost;

            if (r.IsFullTank)
            {
                totalDistance += r.Odometer - anchorOdometer.Value;
                totalLiters += litersSince;
                totalCost += costSince;
                anchorOdometer = r.Odometer;
                litersSince = 0;
                costSince = 0;
            }
        }

        return new FuelStats(totalDistance, totalLiters, totalCost);
    }
}
