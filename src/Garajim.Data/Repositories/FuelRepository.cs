using Garajim.Core.Abstractions;
using Garajim.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Garajim.Data.Repositories;

public sealed class FuelRepository : IFuelRepository
{
    private readonly IDbContextFactory<GarajimDbContext> _factory;

    public FuelRepository(IDbContextFactory<GarajimDbContext> factory) => _factory = factory;

    public async Task<IReadOnlyList<FuelRecord>> GetByVehicleAsync(int vehicleId)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        return await ctx.FuelRecords.AsNoTracking()
            .Where(r => r.VehicleId == vehicleId)
            .OrderByDescending(r => r.Date).ThenByDescending(r => r.Odometer)
            .ToListAsync();
    }

    public async Task<FuelRecord?> GetByIdAsync(int id)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        return await ctx.FuelRecords.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task AddAsync(FuelRecord record)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        ctx.FuelRecords.Add(record);
        await RaiseVehicleOdometerAsync(ctx, record);
        await ctx.SaveChangesAsync(); // kayıt ve araç km güncellemesi tek işlemde
    }

    public async Task UpdateAsync(FuelRecord record)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        var existing = await ctx.FuelRecords.FindAsync(record.Id)
            ?? throw new InvalidOperationException($"Yakıt kaydı bulunamadı: {record.Id}");

        existing.Date = record.Date;
        existing.Odometer = record.Odometer;
        existing.FuelType = record.FuelType;
        existing.Liters = record.Liters;
        existing.UnitPrice = record.UnitPrice;
        existing.TotalCost = record.TotalCost;
        existing.IsFullTank = record.IsFullTank;
        existing.MissedPrevious = record.MissedPrevious;
        existing.Note = record.Note;

        await RaiseVehicleOdometerAsync(ctx, existing);
        await ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        var existing = await ctx.FuelRecords.FindAsync(id);
        if (existing is null) return;

        ctx.FuelRecords.Remove(existing);
        await ctx.SaveChangesAsync();
    }

    private static async Task RaiseVehicleOdometerAsync(GarajimDbContext ctx, FuelRecord record)
    {
        var vehicle = await ctx.Vehicles.FindAsync(record.VehicleId);
        if (vehicle is not null && record.Odometer > vehicle.CurrentOdometer)
            vehicle.CurrentOdometer = record.Odometer;
    }
}
