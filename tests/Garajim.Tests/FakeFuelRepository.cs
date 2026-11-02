using Garajim.Core.Abstractions;
using Garajim.Core.Entities;

namespace Garajim.Tests;

/// <summary>Veritabanı olmadan servis testleri için bellek içi sahte yakıt deposu.</summary>
internal sealed class FakeFuelRepository : IFuelRepository
{
    private readonly List<FuelRecord> _store = new();
    private int _nextId = 1;

    public Task<IReadOnlyList<FuelRecord>> GetByVehicleAsync(int vehicleId)
        => Task.FromResult<IReadOnlyList<FuelRecord>>(_store
            .Where(r => r.VehicleId == vehicleId)
            .OrderByDescending(r => r.Date).ThenByDescending(r => r.Odometer)
            .Select(Clone).ToList());

    public Task<FuelRecord?> GetByIdAsync(int id)
        => Task.FromResult(_store.Where(r => r.Id == id).Select(Clone).FirstOrDefault());

    public Task AddAsync(FuelRecord record)
    {
        record.Id = _nextId++;
        _store.Add(Clone(record));
        return Task.CompletedTask;
    }

    public Task UpdateAsync(FuelRecord record)
    {
        var index = _store.FindIndex(r => r.Id == record.Id);
        _store[index] = Clone(record);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _store.RemoveAll(r => r.Id == id);
        return Task.CompletedTask;
    }

    private static FuelRecord Clone(FuelRecord r) => new()
    {
        Id = r.Id, VehicleId = r.VehicleId, Date = r.Date, Odometer = r.Odometer, FuelType = r.FuelType,
        Liters = r.Liters, UnitPrice = r.UnitPrice, TotalCost = r.TotalCost,
        IsFullTank = r.IsFullTank, MissedPrevious = r.MissedPrevious, Note = r.Note
    };
}
