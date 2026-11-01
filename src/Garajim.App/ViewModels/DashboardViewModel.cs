using CommunityToolkit.Mvvm.ComponentModel;

namespace Garajim.App.ViewModels;

/// <summary>Şimdilik ÖRNEK veri gösterir; gerçek veriler araç ve yakıt modülleri gelince bağlanacak.</summary>
public partial class DashboardViewModel : ObservableObject
{
    public string SelectedVehicleText => "Renault Clio · 35 ABC 123  (örnek veri)";
    public string TotalExpenseText => "₺48.250";
    public string MonthlyFuelText => "₺3.420";
    public string AverageConsumptionText => "6,8 L/100 km";
    public string CostPerKmText => "₺2,14";
}
