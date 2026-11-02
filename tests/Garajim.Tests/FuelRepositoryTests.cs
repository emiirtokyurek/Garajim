using Garajim.Core.Entities;
using Garajim.Core.Enums;
using Garajim.Data;
using Garajim.Data.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Garajim.Tests;

/// <summary>Gerçek repository'yi bellek içi SQLite üzerinde (gerçek migration ile) dener.</summary>
public sealed class FuelRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly VehicleRepository _vehicles;
    private readonly FuelRepository _fuel;

    public FuelRepositoryTests()
    {
        _connection.Open();
        var options = new DbContextOptionsBuilder<GarajimDbContext>().UseSqlite(_connection).Options;
        using (var ctx = new GarajimDbContext(options)) ctx.Database.Migrate();

        var factory = new TestFactory(options);
        _vehicles = new VehicleRepository(factory);
        _fuel = new FuelRepository(factory);
    }

    public void Dispose() => _connection.Dispose();

    private sealed class TestFactory : IDbContextFactory<GarajimDbContext>
    {
        private readonly DbContextOptions<GarajimDbContext> _options;
        public TestFactory(DbContextOptions<GarajimDbContext> options) => _options = options;
        public GarajimDbContext CreateDbContext() => new(_options);
    }

    private async Task<Vehicle> NewVehicleAsync()
    {
        var vehicle = TestVehicles.Valid("35ABC123"); // km: 90000
        await _vehicles.AddAsync(vehicle);
        return vehicle;
    }

    private static FuelRecord Rec(int vehicleId, int day, int odo, decimal total = 1200m) => new()
    {
        VehicleId = vehicleId, Date = new DateOnly(2025, 1, day), Odometer = odo, FuelType = FuelType.Petrol,
        Liters = 40.125m, UnitPrice = 29.9m, TotalCost = total, IsFullTank = true
    };

    [Fact]
    public async Task Yuksek_kilometreli_kayit_aracin_kilometresini_yukseltir()
    {
        var vehicle = await NewVehicleAsync();

        await _fuel.AddAsync(Rec(vehicle.Id, 10, 90500));

        Assert.Equal(90500, (await _vehicles.GetByIdAsync(vehicle.Id))!.CurrentOdometer);
    }

    [Fact]
    public async Task Dusuk_kilometreli_kayit_aracin_kilometresini_dusurmez()
    {
        var vehicle = await NewVehicleAsync();

        await _fuel.AddAsync(Rec(vehicle.Id, 10, 80000));

        Assert.Equal(90000, (await _vehicles.GetByIdAsync(vehicle.Id))!.CurrentOdometer);
    }

    [Fact]
    public async Task Kayitlar_en_yeni_ustte_listelenir_ve_degerler_kayipsiz_saklanir()
    {
        var vehicle = await NewVehicleAsync();
        await _fuel.AddAsync(Rec(vehicle.Id, 5, 90100));
        await _fuel.AddAsync(Rec(vehicle.Id, 20, 90600, total: 1234.56m));

        var list = await _fuel.GetByVehicleAsync(vehicle.Id);

        Assert.Equal(90600, list[0].Odometer);
        Assert.Equal(1234.56m, list[0].TotalCost);
        Assert.Equal(40.125m, list[0].Liters);
    }

    [Fact]
    public async Task Guncelleme_alanlari_yazar()
    {
        var vehicle = await NewVehicleAsync();
        var record = Rec(vehicle.Id, 10, 90500);
        await _fuel.AddAsync(record);

        record.Note = "kış lastiği takıldı";
        record.IsFullTank = false;
        await _fuel.UpdateAsync(record);

        var read = await _fuel.GetByIdAsync(record.Id);
        Assert.Equal("kış lastiği takıldı", read!.Note);
        Assert.False(read.IsFullTank);
    }

    [Fact]
    public async Task Silinen_kayit_artik_okunamaz()
    {
        var vehicle = await NewVehicleAsync();
        var record = Rec(vehicle.Id, 10, 90500);
        await _fuel.AddAsync(record);

        await _fuel.DeleteAsync(record.Id);

        Assert.Null(await _fuel.GetByIdAsync(record.Id));
    }
}
