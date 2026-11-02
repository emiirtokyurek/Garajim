using System.Text.RegularExpressions;
using Garajim.Core.Entities;
using Garajim.Core.Enums;

namespace Garajim.Core.Validation;

public static class VehicleValidator
{
    private static readonly Regex PlateRule = new("^[A-Z0-9]{4,12}$", RegexOptions.Compiled);

    public static ValidationResult Validate(Vehicle vehicle)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(vehicle.Brand)) errors.Add("Marka boş olamaz.");
        else if (vehicle.Brand.Trim().Length > 50) errors.Add("Marka en fazla 50 karakter olabilir.");

        if (string.IsNullOrWhiteSpace(vehicle.Model)) errors.Add("Model boş olamaz.");
        else if (vehicle.Model.Trim().Length > 50) errors.Add("Model en fazla 50 karakter olabilir.");

        var maxYear = DateTime.Today.Year + 1;
        if (vehicle.Year < 1950 || vehicle.Year > maxYear)
            errors.Add($"Model yılı 1950 ile {maxYear} arasında olmalı.");

        if (!string.IsNullOrEmpty(vehicle.EngineInfo) && vehicle.EngineInfo.Trim().Length > 100)
            errors.Add("Motor bilgisi en fazla 100 karakter olabilir.");

        if (!PlateRule.IsMatch(VehiclePlate.Normalize(vehicle.Plate)))
            errors.Add("Plaka 4-12 karakter olmalı ve yalnızca harf/rakam içermeli (Türkçe karakter kullanmayın).");

        if (vehicle.CurrentOdometer < 0) errors.Add("Kilometre negatif olamaz.");

        if (!Enum.IsDefined(vehicle.PrimaryFuelType)) errors.Add("Yakıt türü seçilmeli.");
        else if (vehicle.HasLpg && vehicle.PrimaryFuelType != FuelType.Petrol)
            errors.Add("LPG yalnızca benzinli araçlarda seçilebilir.");

        return errors.Count == 0 ? ValidationResult.Success : new ValidationResult(errors);
    }
}
