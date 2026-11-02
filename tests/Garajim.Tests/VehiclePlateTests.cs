using Garajim.Core.Validation;
using Xunit;

namespace Garajim.Tests;

public class VehiclePlateTests
{
    [Fact]
    public void Normalize_bosluklari_atar_ve_buyuk_harfe_cevirir()
        => Assert.Equal("35ABC123", VehiclePlate.Normalize("35 abc 123"));

    [Fact]
    public void Normalize_tire_ve_bastaki_sondaki_bosluklari_temizler()
        => Assert.Equal("35ABC123", VehiclePlate.Normalize("  35-ABC-123 "));

    [Fact]
    public void Format_turk_plakasini_bosluklu_gosterir()
        => Assert.Equal("35 ABC 123", VehiclePlate.Format("35ABC123"));

    [Fact]
    public void Format_taninmayan_plakayi_oldugu_gibi_birakir()
        => Assert.Equal("XY12", VehiclePlate.Format("XY12"));
}
