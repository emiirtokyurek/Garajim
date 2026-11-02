using Garajim.Core.Abstractions;
using Garajim.Core.Entities;

namespace Garajim.Tests;

/// <summary>Veritabanı olmadan servis testleri için bellek içi sahte depo.</summary>
internal sealed class FakeVehicleRepository : IVehicleRepository
{
    private readonly List<Vehicle> _store = new();
    private int _nextId = 1;

    public Task<IReadOnlyList<Vehicle>> GetAllAsync(bool includeArchived = false)
        => Task.FromResult<IReadOnlyList<Vehicle>>(
            _store.Where(v => includeArchived || !v.IsArchived).Select(Clone).ToList());

    public Task<Vehicle?> GetByIdAsync(int id)
        => Task.FromResult(_store.Where(v => v.Id == id).Select(Clone).FirstOrDefault());

    public Task<bool> PlateExistsAsync(string normalizedPlate, int? excludeId = null)
        => Task.FromResult(_store.Any(v => v.Plate == normalizedPlate && v.Id != excludeId));

    public Task AddAsync(Vehicle vehicle)
    {
        vehicle.Id = _nextId++;
        _store.Add(Clone(vehicle));
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Vehicle vehicle)
    {
        var index = _store.FindIndex(v => v.Id == vehicle.Id);
        var archived = _store[index].IsArchived;
        _store[index] = Clone(vehicle);
        _store[index].IsArchived = archived;
        return Task.CompletedTask;
    }

    public Task SetArchivedAsync(int id, bool archived)
    {
        _store.Single(v => v.Id == id).IsArchived = archived;
        return Task.CompletedTask;
    }

    private static Vehicle Clone(Vehicle v) => new()
    {
        Id = v.Id, Brand = v.Brand, Model = v.Model, Year = v.Year, EngineInfo = v.EngineInfo,
        Plate = v.Plate, CurrentOdometer = v.CurrentOdometer, PrimaryFuelType = v.PrimaryFuelType,
        HasLpg = v.HasLpg, IsArchived = v.IsArchived
    };
}
