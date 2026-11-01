using Garajim.Core.Enums;

namespace Garajim.Core.Entities;

public class Vehicle : BaseEntity
{
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string? EngineInfo { get; set; }
    public string Plate { get; set; } = string.Empty;
    public int CurrentOdometer { get; set; }
    public FuelType PrimaryFuelType { get; set; }
    public bool HasLpg { get; set; }
    public bool IsArchived { get; set; }

    public ICollection<FuelRecord> FuelRecords { get; set; } = new List<FuelRecord>();
    public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
}
