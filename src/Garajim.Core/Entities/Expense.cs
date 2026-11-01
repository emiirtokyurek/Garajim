using Garajim.Core.Enums;

namespace Garajim.Core.Entities;

public class Expense : BaseEntity
{
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    public DateOnly Date { get; set; }
    public ExpenseCategory Category { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
}
