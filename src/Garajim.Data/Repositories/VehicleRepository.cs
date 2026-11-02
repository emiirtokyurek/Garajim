using Garajim.Core.Abstractions;
using Garajim.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Garajim.Data.Repositories;

public sealed class VehicleRepository : IVehicleRepository
{
    private readonly IDbContextFactory<GarajimDbContext> _factory;

    public VehicleRepository(IDbContextFactory<GarajimDbContext> factory) => _factory = factory;

    public async Task<IReadOnlyList<Vehicle>> GetAllAsync(bool includeArchived = false)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        var query = ctx.Vehicles.AsNoTracking();
        if (!includeArchived) query = query.Where(v => !v.IsArchived);
        return await query.OrderBy(v => v.Brand).ThenBy(v => v.Model).ToListAsync();
    }

    public async Task<Vehicle?> GetByIdAsync(int id)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        return await ctx.Vehicles.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<bool> PlateExistsAsync(string normalizedPlate, int? excludeId = null)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        return await ctx.Vehicles.AnyAsync(v => v.Plate == normalizedPlate && (excludeId == null || v.Id != excludeId));
    }

    public async Task AddAsync(Vehicle vehicle)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        ctx.Vehicles.Add(vehicle);
        await ctx.SaveChangesAsync();
    }

    public async Task UpdateAsync(Vehicle vehicle)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        var existing = await ctx.Vehicles.FindAsync(vehicle.Id)
            ?? throw new InvalidOperationException($"Araç bulunamadı: {vehicle.Id}");

        existing.Brand = vehicle.Brand;
        existing.Model = vehicle.Model;
        existing.Year = vehicle.Year;
        existing.EngineInfo = vehicle.EngineInfo;
        existing.Plate = vehicle.Plate;
        existing.CurrentOdometer = vehicle.CurrentOdometer;
        existing.PrimaryFuelType = vehicle.PrimaryFuelType;
        existing.HasLpg = vehicle.HasLpg;

        await ctx.SaveChangesAsync();
    }

    public async Task SetArchivedAsync(int id, bool archived)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        var existing = await ctx.Vehicles.FindAsync(id)
            ?? throw new InvalidOperationException($"Araç bulunamadı: {id}");

        existing.IsArchived = archived;
        await ctx.SaveChangesAsync();
    }
}
