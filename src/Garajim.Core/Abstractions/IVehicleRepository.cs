using Garajim.Core.Entities;

namespace Garajim.Core.Abstractions;

public interface IVehicleRepository
{
    Task<IReadOnlyList<Vehicle>> GetAllAsync(bool includeArchived = false);
    Task<Vehicle?> GetByIdAsync(int id);

    /// <param name="normalizedPlate">VehiclePlate.Normalize ile temizlenmiş plaka.</param>
    /// <param name="excludeId">Düzenlemede aracın kendi kaydını saymamak için.</param>
    Task<bool> PlateExistsAsync(string normalizedPlate, int? excludeId = null);

    /// <summary>Kayıt sonrası vehicle.Id dolar.</summary>
    Task AddAsync(Vehicle vehicle);

    /// <summary>Sadece düzenlenebilir alanları günceller (marka, model, yıl, motor, plaka, km, yakıt, LPG).</summary>
    Task UpdateAsync(Vehicle vehicle);

    Task SetArchivedAsync(int id, bool archived);
}
