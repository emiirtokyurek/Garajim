using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Garajim.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly DashboardViewModel _dashboard;
    private readonly VehiclesViewModel _vehicles;

    [ObservableProperty]
    private object? _currentViewModel;

    public MainViewModel(DashboardViewModel dashboard, VehiclesViewModel vehicles)
    {
        _dashboard = dashboard;
        _vehicles = vehicles;
        CurrentViewModel = dashboard;
    }

    [RelayCommand]
    private async Task NavigateAsync(string? page)
    {
        switch (page)
        {
            case "Vehicles":
                CurrentViewModel = _vehicles;
                await _vehicles.LoadAsync();
                break;
            case "Fuel": CurrentViewModel = Soon("Yakıt"); break;
            case "Maintenance": CurrentViewModel = Soon("Bakım"); break;
            case "Expenses": CurrentViewModel = Soon("Giderler"); break;
            case "Reports": CurrentViewModel = Soon("Raporlar"); break;
            case "Reminders": CurrentViewModel = Soon("Hatırlatmalar"); break;
            default:
                CurrentViewModel = _dashboard;
                await _dashboard.LoadAsync(); // araçlar sayfasında değişiklik olduysa yenilenir
                break;
        }
    }

    private static PlaceholderViewModel Soon(string title)
        => new(title, "Bu modül sonraki aşamalarda eklenecek.");
}
