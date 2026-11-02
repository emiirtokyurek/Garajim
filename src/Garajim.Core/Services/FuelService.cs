using Garajim.Core.Abstractions;
using Garajim.Core.Entities;
using Garajim.Core.Validation;

namespace Garajim.Core.Services;

/// <summary>Yakıt kaydı iş kuralları. Arayüz yalnızca bunu çağırır.</summary>
public sealed class FuelService
{
    private readonly IFuelRepository _fuel;
    private readonly IVehicleRepository _vehicles;

    public FuelService(IFuelRepository fuel, IVehicleRepository vehicles)
    {
        _fuel = fuel;
        _vehicles = vehicles;
    }

    public Task<IReadOnlyList<FuelRecord>> GetRecordsAsync(int vehicleId) => _fuel.GetByVehicleAsync(vehicleId);

    public async Task<FuelSummary> GetSummaryAsync(int vehicleId, DateOnly? today = null)
    {
        var records = await _fuel.GetByVehicleAsync(vehicleId);
        return FuelSummaryCalculator.Calculate(records, today ?? DateOnly.FromDateTime(DateTime.Today));
    }

    public async Task<ValidationResult> AddAsync(FuelRecord record)
    {
        var vehicle = await _vehicles.GetByIdAsync(record.VehicleId);
        if (vehicle is null) return Fail("Araç bulunamadı.");
        if (vehicle.IsArchived) return Fail("Arşivlenmiş araca yakıt kaydı eklenemez.");

        var errors = await CheckAsync(record, vehicle);
        if (errors.Count > 0) return new ValidationResult(errors);

        await _fuel.AddAsync(record);
        return ValidationResult.Success;
    }

    public async Task<ValidationResult> UpdateAsync(FuelRecord record)
    {
        var existing = await _fuel.GetByIdAsync(record.Id);
        if (existing is null) return Fail("Kayıt bulunamadı.");

        var vehicle = await _vehicles.GetByIdAsync(existing.VehicleId);
        if (vehicle is null) return Fail("Araç bulunamadı.");

        record.VehicleId = existing.VehicleId; // kaydın aracı değiştirilemez

        var errors = await CheckAsync(record, vehicle);
        if (errors.Count > 0) return new ValidationResult(errors);

        await _fuel.UpdateAsync(record);
        return ValidationResult.Success;
    }

    public Task DeleteAsync(int id) => _fuel.DeleteAsync(id);

    private async Task<List<string>> CheckAsync(FuelRecord record, Vehicle vehicle)
    {
        record.Note = string.IsNullOrWhiteSpace(record.Note) ? null : record.Note.Trim();
        FuelRecordCompleter.Complete(record);

        // Komşu kayıtlar tarihe göre bulunur: kilometre zamanla azalamaz.
        var others = (await _fuel.GetByVehicleAsync(vehicle.Id)).Where(r => r.Id != record.Id).ToList();
        int? previous = others.Where(r => r.Date < record.Date).Select(r => (int?)r.Odometer).Max();
        int? next = others.Where(r => r.Date > record.Date).Select(r => (int?)r.Odometer).Min();

        var errors = FuelRecordValidator.Validate(record, previous, next).Errors.ToList();
        if (!FuelCompatibility.IsAllowed(vehicle, record.FuelType))
            errors.Add("Bu yakıt türü bu araca eklenemez.");
        return errors;
    }

    private static ValidationResult Fail(string message) => new(new[] { message });
}
