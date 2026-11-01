using Garajim.Core.Enums;

namespace Garajim.Core.Entities;

public class MaintenanceRecord : BaseEntity
{
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    public DateOnly Date { get; set; }
    public int Odometer { get; set; }
    public MaintenanceType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal PartsCost { get; set; }
    public decimal LaborCost { get; set; }
    public int? NextDueOdometer { get; set; }
    public DateOnly? NextDueDate { get; set; }

    /// <summary>Veritabanında tutulmaz, hesaplanır.</summary>
    public decimal TotalCost => PartsCost + LaborCost;
}
