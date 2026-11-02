using Garajim.Core.Entities;

namespace Garajim.Core.Abstractions;

public interface IFuelRepository
{
    /// <summary>Aracın yakıt kayıtları, en yeni en üstte (tarih, sonra kilometre).</summary>
    Task<IReadOnlyList<FuelRecord>> GetByVehicleAsync(int vehicleId);

    Task<FuelRecord?> GetByIdAsync(int id);

    /// <summary>Kaydeder; kayıt kilometresi aracın güncel kilometresinden büyükse aracı da aynı işlemde günceller.</summary>
    Task AddAsync(FuelRecord record);

    /// <summary>Tarih, km, yakıt türü, litre, fiyat, tutar, tam depo, kaçırılan dolum ve notu günceller.</summary>
    Task UpdateAsync(FuelRecord record);

    Task DeleteAsync(int id);
}
