using Garajim.Core.Entities;
using Garajim.Core.Enums;

namespace Garajim.Tests;

internal static class TestVehicles
{
    public static Vehicle Valid(string plate = "35 ABC 123") => new()
    {
        Brand = "Renault", Model = "Clio", Year = 2018, EngineInfo = "1.2 TCe",
        Plate = plate, CurrentOdometer = 90000, PrimaryFuelType = FuelType.Petrol
    };
}
