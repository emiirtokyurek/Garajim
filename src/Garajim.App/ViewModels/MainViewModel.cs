using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Garajim.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly DashboardViewModel _dashboard;

    [ObservableProperty]
    private object? _currentViewModel;

    public MainViewModel(DashboardViewModel dashboard)
    {
        _dashboard = dashboard;
        CurrentViewModel = dashboard;
    }

    [RelayCommand]
    private void Navigate(string? page)
    {
        CurrentViewModel = page switch
        {
            "Dashboard" => _dashboard,
            "Vehicles" => Soon("Araçlar"),
            "Fuel" => Soon("Yakıt"),
            "Maintenance" => Soon("Bakım"),
            "Expenses" => Soon("Giderler"),
            "Reports" => Soon("Raporlar"),
            "Reminders" => Soon("Hatırlatmalar"),
            _ => _dashboard
        };
    }

    private static PlaceholderViewModel Soon(string title)
        => new(title, "Bu modül sonraki aşamalarda eklenecek.");
}
