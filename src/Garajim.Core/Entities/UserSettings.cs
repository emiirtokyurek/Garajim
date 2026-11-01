namespace Garajim.Core.Entities;

/// <summary>Tek satırlık tablo (Id = 1).</summary>
public class UserSettings : BaseEntity
{
    public string Theme { get; set; } = "Dark";
    public string Currency { get; set; } = "TRY";
    public string DistanceUnit { get; set; } = "km";
    public int? DefaultVehicleId { get; set; }
    public DateTime? LastBackupAt { get; set; }
}
