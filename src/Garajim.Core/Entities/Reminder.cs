using Garajim.Core.Enums;

namespace Garajim.Core.Entities;

public class Reminder : BaseEntity
{
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public ReminderType Type { get; set; }
    public DateOnly? DueDate { get; set; }
    public int? DueOdometer { get; set; }
    public bool IsCompleted { get; set; }
}
