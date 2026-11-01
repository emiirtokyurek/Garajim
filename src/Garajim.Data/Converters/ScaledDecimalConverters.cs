using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Garajim.Data.Converters;

/// <summary>Para: 12.34 TL → 1234 (kuruş, INTEGER). SQLite'ta toplama/sıralama güvenli olur.</summary>
public sealed class MoneyConverter : ValueConverter<decimal, long>
{
    public MoneyConverter() : base(
        v => (long)Math.Round(v * 100m, MidpointRounding.AwayFromZero),
        v => v / 100m)
    { }
}

/// <summary>Litre ve birim fiyat: 3 ondalık hane (40.125 → 40125).</summary>
public sealed class QuantityConverter : ValueConverter<decimal, long>
{
    public QuantityConverter() : base(
        v => (long)Math.Round(v * 1000m, MidpointRounding.AwayFromZero),
        v => v / 1000m)
    { }
}
