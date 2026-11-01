using Garajim.Core.Entities;
using Garajim.Core.Enums;
using Garajim.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Garajim.Tests;

/// <summary>Gerçek migration'ı bellek içi SQLite üzerinde çalıştırır; diske dokunmaz.</summary>
public sealed class DatabaseTests : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly DbContextOptions<GarajimDbContext> _options;

    public DatabaseTests()
    {
        _connection.Open();
        _options = new DbContextOptionsBuilder<GarajimDbContext>().UseSqlite(_connection).Options;
        using var ctx = new GarajimDbContext(_options);
        ctx.Database.Migrate();
    }

    public void Dispose() => _connection.Dispose();

    private GarajimDbContext NewContext() => new(_options);

    private static Vehicle NewVehicle(string plate) => new()
    {
        Brand = "Renault", Model = "Clio", Year = 2018, Plate = plate,
        PrimaryFuelType = FuelType.Petrol, CurrentOdometer = 90000
    };

    [Fact]
    public void Para_ve_litre_degerleri_kayipsiz_saklanir()
    {
        int vehicleId;
        using (var ctx = NewContext())
        {
            var vehicle = NewVehicle("35 ABC 123");
            vehicle.FuelRecords.Add(new FuelRecord
            {
                Date = new DateOnly(2026, 1, 15), Odometer = 90500, FuelType = FuelType.Petrol,
                Liters = 40.125m, UnitPrice = 30.765m, TotalCost = 1234.56m
            });
            ctx.Vehicles.Add(vehicle);
            ctx.SaveChanges();
            vehicleId = vehicle.Id;
        }

        using var read = NewContext();
        var record = read.FuelRecords.Single(f => f.VehicleId == vehicleId);

        Assert.Equal(40.125m, record.Liters);
        Assert.Equal(30.765m, record.UnitPrice);
        Assert.Equal(1234.56m, record.TotalCost);
        Assert.Equal(new DateOnly(2026, 1, 15), record.Date);
    }

    [Fact]
    public void Ayni_plaka_iki_kez_eklenemez()
    {
        using var ctx = NewContext();
        ctx.Vehicles.Add(NewVehicle("35 ABC 123"));
        ctx.SaveChanges();

        ctx.Vehicles.Add(NewVehicle("35 ABC 123"));
        Assert.Throws<DbUpdateException>(() => ctx.SaveChanges());
    }

    [Fact]
    public void Kaydi_olan_arac_silinemez()
    {
        int vehicleId;
        using (var ctx = NewContext())
        {
            var vehicle = NewVehicle("06 XYZ 789");
            vehicle.Expenses.Add(new Expense
            {
                Date = new DateOnly(2026, 2, 1), Category = ExpenseCategory.Kasko, Amount = 5000m
            });
            ctx.Vehicles.Add(vehicle);
            ctx.SaveChanges();
            vehicleId = vehicle.Id;
        }

        using var del = NewContext();
        del.Vehicles.Remove(del.Vehicles.Single(v => v.Id == vehicleId));

        Assert.Throws<DbUpdateException>(() => del.SaveChanges());
    }

    [Fact]
    public void Varsayilan_ayar_kaydi_hazir_gelir()
    {
        using var ctx = NewContext();
        var settings = ctx.UserSettings.Single();

        Assert.Equal("TRY", settings.Currency);
        Assert.Equal("Dark", settings.Theme);
    }

    [Fact]
    public void Olusturma_ve_guncelleme_tarihi_otomatik_yazilir()
    {
        using var ctx = NewContext();
        var vehicle = NewVehicle("34 TEST 01");
        ctx.Vehicles.Add(vehicle);
        ctx.SaveChanges();

        Assert.NotEqual(default, vehicle.CreatedAt);
        Assert.Equal(vehicle.CreatedAt, vehicle.UpdatedAt);
    }
}
