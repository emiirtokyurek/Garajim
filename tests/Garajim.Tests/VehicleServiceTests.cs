using Garajim.Core.Services;
using Xunit;

namespace Garajim.Tests;

public class VehicleServiceTests
{
    private readonly VehicleService _service = new(new FakeVehicleRepository());

    [Fact]
    public async Task Ekleme_plakayi_normallestirir_ve_kaydeder()
    {
        var vehicle = TestVehicles.Valid("35 abc 123");

        var result = await _service.AddAsync(vehicle);

        Assert.True(result.IsValid);
        Assert.Equal("35ABC123", vehicle.Plate);
        Assert.NotEqual(0, vehicle.Id);
        Assert.Single(await _service.GetActiveAsync());
    }

    [Fact]
    public async Task Ayni_plaka_farkli_yazimla_da_reddedilir()
    {
        await _service.AddAsync(TestVehicles.Valid("35 ABC 123"));

        var result = await _service.AddAsync(TestVehicles.Valid("35abc123"));

        Assert.False(result.IsValid);
        Assert.Contains("plaka", result.Errors[0]);
        Assert.Single(await _service.GetAllAsync());
    }

    [Fact]
    public async Task Gecersiz_arac_kaydedilmez()
    {
        var vehicle = TestVehicles.Valid();
        vehicle.Brand = "";

        var result = await _service.AddAsync(vehicle);

        Assert.False(result.IsValid);
        Assert.Empty(await _service.GetAllAsync());
    }

    [Fact]
    public async Task Duzenlemede_aracin_kendi_plakasi_sorun_olmaz()
    {
        var vehicle = TestVehicles.Valid();
        await _service.AddAsync(vehicle);

        vehicle.Brand = "Dacia";
        var result = await _service.UpdateAsync(vehicle);

        Assert.True(result.IsValid);
        Assert.Equal("Dacia", (await _service.GetByIdAsync(vehicle.Id))!.Brand);
    }

    [Fact]
    public async Task Duzenlemede_baska_aracin_plakasi_reddedilir()
    {
        await _service.AddAsync(TestVehicles.Valid("35 ABC 123"));
        var second = TestVehicles.Valid("06 XYZ 789");
        await _service.AddAsync(second);

        second.Plate = "35 ABC 123";
        var result = await _service.UpdateAsync(second);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Olmayan_arac_guncellenemez()
    {
        var vehicle = TestVehicles.Valid();
        vehicle.Id = 999;

        var result = await _service.UpdateAsync(vehicle);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Arsivlenen_arac_aktif_listeden_cikar_ama_silinmez()
    {
        var vehicle = TestVehicles.Valid();
        await _service.AddAsync(vehicle);

        await _service.ArchiveAsync(vehicle.Id);

        Assert.Empty(await _service.GetActiveAsync());
        Assert.Single(await _service.GetAllAsync());
    }

    [Fact]
    public async Task Arsivden_geri_alinan_arac_tekrar_listelenir()
    {
        var vehicle = TestVehicles.Valid();
        await _service.AddAsync(vehicle);
        await _service.ArchiveAsync(vehicle.Id);

        await _service.RestoreAsync(vehicle.Id);

        Assert.Single(await _service.GetActiveAsync());
    }
}
