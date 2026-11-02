using Garajim.Core.Entities;
using Garajim.Core.Enums;
using Garajim.Core.Services;
using Xunit;

namespace Garajim.Tests;

public class FuelServiceTests
{
    private readonly FakeVehicleRepository _vehicles = new();
    private readonly FuelService _service;

    public FuelServiceTests() => _service = new FuelService(new FakeFuelRepository(), _vehicles);

    private async Task<Vehicle> NewVehicleAsync()
    {
        var vehicle = TestVehicles.Valid("35ABC123");
        await _vehicles.AddAsync(vehicle);
        return vehicle;
    }

    private static FuelRecord Rec(int vehicleId, int month, int day, int odo, FuelType type = FuelType.Petrol)
        => new()
        {
            VehicleId = vehicleId, Date = new DateOnly(2025, month, day), Odometer = odo,
            FuelType = type, Liters = 40, UnitPrice = 30, IsFullTank = true
        };

    [Fact]
    public async Task Gecerli_kayit_eklenir_ve_eksik_toplam_hesaplanir()
    {
        var vehicle = await NewVehicleAsync();
        var record = Rec(vehicle.Id, 1, 10, 90500);

        var result = await _service.AddAsync(record);

        Assert.True(result.IsValid);
        Assert.Equal(1200m, record.TotalCost);
        Assert.Single(await _service.GetRecordsAsync(vehicle.Id));
    }

    [Fact]
    public async Task Arsivlenmis_araca_kayit_eklenemez()
    {
        var vehicle = await NewVehicleAsync();
        await _vehicles.SetArchivedAsync(vehicle.Id, true);

        var result = await _service.AddAsync(Rec(vehicle.Id, 1, 10, 90500));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Araca_uymayan_yakit_turu_reddedilir()
    {
        var vehicle = await NewVehicleAsync(); // benzinli, LPG'siz

        var result = await _service.AddAsync(Rec(vehicle.Id, 1, 10, 90500, FuelType.Diesel));

        Assert.False(result.IsValid);
        Assert.Empty(await _service.GetRecordsAsync(vehicle.Id));
    }

    [Fact]
    public async Task Km_onceki_tarihli_kayittan_kucukse_reddedilir()
    {
        var vehicle = await NewVehicleAsync();
        await _service.AddAsync(Rec(vehicle.Id, 1, 10, 91000));

        var result = await _service.AddAsync(Rec(vehicle.Id, 2, 10, 90900));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Km_sonraki_tarihli_kayittan_buyukse_reddedilir()
    {
        var vehicle = await NewVehicleAsync();
        await _service.AddAsync(Rec(vehicle.Id, 3, 10, 92000));

        var result = await _service.AddAsync(Rec(vehicle.Id, 2, 10, 92500));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Guncelleme_notu_temizleyip_kaydeder()
    {
        var vehicle = await NewVehicleAsync();
        var record = Rec(vehicle.Id, 1, 10, 90500);
        await _service.AddAsync(record);

        record.Note = "  yolculuk  ";
        var result = await _service.UpdateAsync(record);

        Assert.True(result.IsValid);
        Assert.Equal("yolculuk", (await _service.GetRecordsAsync(vehicle.Id))[0].Note);
    }

    [Fact]
    public async Task Olmayan_kayit_guncellenemez()
    {
        var vehicle = await NewVehicleAsync();
        var record = Rec(vehicle.Id, 1, 10, 90500);
        record.Id = 999;

        Assert.False((await _service.UpdateAsync(record)).IsValid);
    }

    [Fact]
    public async Task Silinen_kayit_listeden_kalkar()
    {
        var vehicle = await NewVehicleAsync();
        var record = Rec(vehicle.Id, 1, 10, 90500);
        await _service.AddAsync(record);

        await _service.DeleteAsync(record.Id);

        Assert.Empty(await _service.GetRecordsAsync(vehicle.Id));
    }

    [Fact]
    public async Task Ozet_servis_uzerinden_hesaplanir()
    {
        var vehicle = await NewVehicleAsync();
        await _service.AddAsync(Rec(vehicle.Id, 1, 10, 91000));
        var second = Rec(vehicle.Id, 1, 20, 91500);
        second.Liters = 35;
        await _service.AddAsync(second);

        var summary = await _service.GetSummaryAsync(vehicle.Id, new DateOnly(2025, 1, 25));

        Assert.Equal(2250m, summary.CurrentMonthCost);
        Assert.Single(summary.Consumption);
        Assert.Equal(7m, summary.Consumption[0].Stats.LitersPer100Km);
    }
}
