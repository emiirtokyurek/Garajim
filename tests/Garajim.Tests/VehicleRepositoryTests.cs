using Garajim.Data;
using Garajim.Data.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Garajim.Tests;

/// <summary>Gerçek repository'yi bellek içi SQLite üzerinde (gerçek migration ile) dener.</summary>
public sealed class VehicleRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly VehicleRepository _repository;

    public VehicleRepositoryTests()
    {
        _connection.Open();
        var options = new DbContextOptionsBuilder<GarajimDbContext>().UseSqlite(_connection).Options;
        using (var ctx = new GarajimDbContext(options)) ctx.Database.Migrate();
        _repository = new VehicleRepository(new TestFactory(options));
    }

    public void Dispose() => _connection.Dispose();

    private sealed class TestFactory : IDbContextFactory<GarajimDbContext>
    {
        private readonly DbContextOptions<GarajimDbContext> _options;
        public TestFactory(DbContextOptions<GarajimDbContext> options) => _options = options;
        public GarajimDbContext CreateDbContext() => new(_options);
    }

    [Fact]
    public async Task Eklenen_arac_okunabilir()
    {
        var vehicle = TestVehicles.Valid("35ABC123");
        await _repository.AddAsync(vehicle);

        var read = await _repository.GetByIdAsync(vehicle.Id);

        Assert.NotNull(read);
        Assert.Equal("Renault", read!.Brand);
        Assert.Equal(90000, read.CurrentOdometer);
    }

    [Fact]
    public async Task Arsivlenen_arac_varsayilan_listede_gorunmez()
    {
        var vehicle = TestVehicles.Valid("35ABC123");
        await _repository.AddAsync(vehicle);

        await _repository.SetArchivedAsync(vehicle.Id, true);

        Assert.Empty(await _repository.GetAllAsync());
        Assert.Single(await _repository.GetAllAsync(includeArchived: true));
    }

    [Fact]
    public async Task Guncelleme_duzenlenebilir_alanlari_yazar()
    {
        var vehicle = TestVehicles.Valid("35ABC123");
        await _repository.AddAsync(vehicle);

        vehicle.Brand = "Dacia";
        vehicle.CurrentOdometer = 95000;
        await _repository.UpdateAsync(vehicle);

        var read = await _repository.GetByIdAsync(vehicle.Id);
        Assert.Equal("Dacia", read!.Brand);
        Assert.Equal(95000, read.CurrentOdometer);
    }

    [Fact]
    public async Task Plaka_kontrolu_kendi_kaydini_haric_tutabilir()
    {
        var vehicle = TestVehicles.Valid("35ABC123");
        await _repository.AddAsync(vehicle);

        Assert.True(await _repository.PlateExistsAsync("35ABC123"));
        Assert.False(await _repository.PlateExistsAsync("35ABC123", excludeId: vehicle.Id));
    }
}
