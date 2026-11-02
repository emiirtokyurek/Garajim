using Garajim.Core.Entities;
using Garajim.Core.Enums;

namespace Garajim.Core.Validation;

/// <summary>Hangi araca hangi yakıt kaydı girilebilir?</summary>
public static class FuelCompatibility
{
    public static IReadOnlyList<FuelType> AllowedFor(Vehicle vehicle) => vehicle.PrimaryFuelType switch
    {
        FuelType.Petrol => vehicle.HasLpg ? new[] { FuelType.Petrol, FuelType.Lpg } : new[] { FuelType.Petrol },
        FuelType.Diesel => new[] { FuelType.Diesel },
        FuelType.Hybrid => new[] { FuelType.Petrol, FuelType.Electric }, // şarj edilebilir hibritler için
        FuelType.Electric => new[] { FuelType.Electric },
        FuelType.Lpg => new[] { FuelType.Lpg },
        _ => Array.Empty<FuelType>()
    };

    public static bool IsAllowed(Vehicle vehicle, FuelType fuelType) => AllowedFor(vehicle).Contains(fuelType);
}
