using Garajim.Core.Entities;

namespace Garajim.Core.Services;

public static class VehicleSelector
{
    /// <summary>Tercih edilen araç listede varsa onu, yoksa ilk aracı, liste boşsa null döner.</summary>
    public static Vehicle? Choose(IReadOnlyList<Vehicle> vehicles, int? preferredId)
        => vehicles.FirstOrDefault(v => v.Id == preferredId) ?? vehicles.FirstOrDefault();
}
