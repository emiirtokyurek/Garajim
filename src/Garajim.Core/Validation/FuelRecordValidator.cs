using Garajim.Core.Entities;

namespace Garajim.Core.Validation;

public static class FuelRecordValidator
{
    private const decimal MaxLiters = 200m;

    /// <param name="previousOdometer">Tarihçede bu kayıttan önceki tarihli kayıtların en yüksek km'si (varsa).</param>
    /// <param name="nextOdometer">Tarihçede bu kayıttan sonraki tarihli kayıtların en düşük km'si (varsa).</param>
    public static ValidationResult Validate(FuelRecord record, int? previousOdometer = null, int? nextOdometer = null)
    {
        var errors = new List<string>();

        if (record.Liters <= 0) errors.Add("Litre sıfırdan büyük olmalı.");
        else if (record.Liters > MaxLiters) errors.Add($"Litre değeri olağandışı yüksek (en fazla {MaxLiters:N0}).");

        if (record.UnitPrice <= 0) errors.Add("Birim fiyat sıfırdan büyük olmalı.");
        if (record.TotalCost <= 0) errors.Add("Toplam tutar sıfırdan büyük olmalı.");
        if (record.Odometer < 0) errors.Add("Kilometre negatif olamaz.");

        // Yazım hatalarını yakalar: litre x birim fiyat, toplam tutarla (küçük yuvarlama farkı dışında) tutmalı.
        if (record.Liters > 0 && record.UnitPrice > 0 && record.TotalCost > 0)
        {
            var expected = record.Liters * record.UnitPrice;
            var tolerance = Math.Max(1m, record.TotalCost * 0.01m);
            if (Math.Abs(expected - record.TotalCost) > tolerance)
                errors.Add($"Litre × birim fiyat ({expected:N2}) toplam tutarla ({record.TotalCost:N2}) uyuşmuyor.");
        }

        if (previousOdometer is { } prev && record.Odometer < prev)
            errors.Add($"Kilometre önceki kayıttan ({prev:N0}) küçük olamaz.");
        if (nextOdometer is { } next && record.Odometer > next)
            errors.Add($"Kilometre sonraki kayıttan ({next:N0}) büyük olamaz.");

        if (record.Date > DateOnly.FromDateTime(DateTime.Today))
            errors.Add("Tarih gelecekte olamaz.");

        if (record.Note is { Length: > 500 }) errors.Add("Not en fazla 500 karakter olabilir.");

        return errors.Count == 0 ? ValidationResult.Success : new ValidationResult(errors);
    }
}
