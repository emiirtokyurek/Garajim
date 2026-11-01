using Garajim.Core.Entities;

namespace Garajim.Core.Validation;

public static class FuelRecordValidator
{
    /// <param name="previousOdometer">Tarihçede bu kayıttan hemen önceki kaydın km'si (varsa).</param>
    /// <param name="nextOdometer">Tarihçede bu kayıttan hemen sonraki kaydın km'si (varsa).</param>
    public static ValidationResult Validate(FuelRecord record, int? previousOdometer = null, int? nextOdometer = null)
    {
        var errors = new List<string>();

        if (record.Liters <= 0) errors.Add("Litre sıfırdan büyük olmalı.");
        if (record.UnitPrice <= 0) errors.Add("Birim fiyat sıfırdan büyük olmalı.");
        if (record.TotalCost <= 0) errors.Add("Toplam tutar sıfırdan büyük olmalı.");
        if (record.Odometer < 0) errors.Add("Kilometre negatif olamaz.");

        if (previousOdometer is { } prev && record.Odometer < prev)
            errors.Add($"Kilometre önceki kayıttan ({prev:N0}) küçük olamaz.");
        if (nextOdometer is { } next && record.Odometer > next)
            errors.Add($"Kilometre sonraki kayıttan ({next:N0}) büyük olamaz.");

        if (record.Date > DateOnly.FromDateTime(DateTime.Today))
            errors.Add("Tarih gelecekte olamaz.");

        return new ValidationResult(errors);
    }
}
