using Garajim.Core.Entities;
using Garajim.Core.Services;
using Xunit;

namespace Garajim.Tests;

public class VehicleSelectorTests
{
    private static readonly IReadOnlyList<Vehicle> Two = new[]
    {
        new Vehicle { Id = 1, Brand = "Renault" },
        new Vehicle { Id = 2, Brand = "Dacia" }
    };

    [Fact]
    public void Tercih_edilen_arac_listede_varsa_o_secilir()
        => Assert.Equal(2, VehicleSelector.Choose(Two, 2)!.Id);

    [Fact]
    public void Tercih_edilen_arac_listede_yoksa_ilk_arac_secilir()
        => Assert.Equal(1, VehicleSelector.Choose(Two, 99)!.Id);

    [Fact]
    public void Tercih_yoksa_ilk_arac_secilir()
        => Assert.Equal(1, VehicleSelector.Choose(Two, null)!.Id);

    [Fact]
    public void Liste_bossa_null_doner()
        => Assert.Null(VehicleSelector.Choose(Array.Empty<Vehicle>(), 1));
}
