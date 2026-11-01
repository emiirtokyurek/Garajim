using Garajim.Core.Enums;

namespace Garajim.Core.Entities;

public class FuelRecord : BaseEntity
{
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    public DateOnly Date { get; set; }
    public int Odometer { get; set; }
    public FuelType FuelType { get; set; }
    public decimal Liters { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalCost { get; set; }

    /// <summary>Depo tamamen dolduruldu mu? Tüketim hesabı için şart.</summary>
    public bool IsFullTank { get; set; } = true;

    /// <summary>Bir önceki dolumu kaydetmeyi kaçırdıysan işaretle; hesap bu noktada zinciri keser.</summary>
    public bool MissedPrevious { get; set; }

    public string? Note { get; set; }
}
