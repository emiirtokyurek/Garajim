namespace Garajim.Core.Enums;

/// <summary>Yakıt ve bakım dışındaki giderler. (Yakıt/bakım kendi tablolarında tutulur, çift sayım olmasın.)</summary>
public enum ExpenseCategory
{
    TrafficInsurance = 1,
    Kasko = 2,
    Inspection = 3,
    Tax = 4,
    Other = 99
}
