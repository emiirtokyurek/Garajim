using Garajim.Core.Abstractions;
using Garajim.Core.Entities;
using Garajim.Core.Validation;

namespace Garajim.Core.Services;

/// <summary>Araç iş kuralları: temizleme, doğrulama, plaka benzersizliği, arşivleme. Arayüz bunu çağırır.</summary>
public sealed class VehicleService
{
    private readonly IVehicleRepository _repository;

    public VehicleService(IVehicleRepository repository) => _repository = repository;

    public Task<IReadOnlyList<Vehicle>> GetActiveAsync() => _repository.GetAllAsync(includeArchived: false);
    public Task<IReadOnlyList<Vehicle>> GetAllAsync() => _repository.GetAllAsync(includeArchived: true);
    public Task<Vehicle?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

    public async Task<ValidationResult> AddAsync(Vehicle vehicle)
    {
        Clean(vehicle);

        var errors = VehicleValidator.Validate(vehicle).Errors.ToList();
        if (errors.Count == 0 && await _repository.PlateExistsAsync(vehicle.Plate))
            errors.Add("Bu plaka zaten kayıtlı (arşivlenmiş bir araç olabilir).");
        if (errors.Count > 0) return new ValidationResult(errors);

        await _repository.AddAsync(vehicle);
        return ValidationResult.Success;
    }

    public async Task<ValidationResult> UpdateAsync(Vehicle vehicle)
    {
        Clean(vehicle);

        var errors = VehicleValidator.Validate(vehicle).Errors.ToList();
        if (errors.Count == 0)
        {
            if (await _repository.GetByIdAsync(vehicle.Id) is null)
                errors.Add("Araç bulunamadı.");
            else if (await _repository.PlateExistsAsync(vehicle.Plate, excludeId: vehicle.Id))
                errors.Add("Bu plaka başka bir araçta kayıtlı (arşivlenmiş olabilir).");
        }
        if (errors.Count > 0) return new ValidationResult(errors);

        await _repository.UpdateAsync(vehicle);
        return ValidationResult.Success;
    }

    /// <summary>Araç silinmez, arşive alınır; geçmiş kayıtlar korunur.</summary>
    public Task ArchiveAsync(int id) => _repository.SetArchivedAsync(id, true);

    public Task RestoreAsync(int id) => _repository.SetArchivedAsync(id, false);

    private static void Clean(Vehicle vehicle)
    {
        vehicle.Brand = (vehicle.Brand ?? string.Empty).Trim();
        vehicle.Model = (vehicle.Model ?? string.Empty).Trim();
        vehicle.EngineInfo = string.IsNullOrWhiteSpace(vehicle.EngineInfo) ? null : vehicle.EngineInfo.Trim();
        vehicle.Plate = VehiclePlate.Normalize(vehicle.Plate);
    }
}
