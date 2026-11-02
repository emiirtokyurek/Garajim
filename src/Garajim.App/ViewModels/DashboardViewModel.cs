using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Garajim.App.Services;
using Garajim.Core.Entities;
using Garajim.Core.Enums;
using Garajim.Core.Services;
using Garajim.Core.Validation;

namespace Garajim.App.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private const string NoData = "—";

    private readonly VehicleService _vehicleService;
    private readonly VehicleSelection _selection;
    private bool _isLoading;

    public DashboardViewModel(VehicleService vehicleService, VehicleSelection selection)
    {
        _vehicleService = vehicleService;
        _selection = selection;
    }

    public ObservableCollection<VehicleItem> Vehicles { get; } = new();

    [ObservableProperty] private VehicleItem? _selectedVehicle;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasNoVehicles))]
    private bool _hasVehicles;

    [ObservableProperty] private string _headerText = "Araç seçilmedi";
    [ObservableProperty] private string _detailText = string.Empty;

    public bool HasNoVehicles => !HasVehicles;

    // Gerçek veriler yakıt/bakım/gider modülleri gelince bağlanacak.
    public string TotalExpenseText => NoData;
    public string MonthlyFuelText => NoData;
    public string AverageConsumptionText => NoData;
    public string CostPerKmText => NoData;

    public async Task LoadAsync()
    {
        try
        {
            var list = await _vehicleService.GetActiveAsync();

            _isLoading = true;
            Vehicles.Clear();
            foreach (var vehicle in list) Vehicles.Add(new VehicleItem(vehicle));
            HasVehicles = Vehicles.Count > 0;

            var chosen = VehicleSelector.Choose(list, _selection.Current?.Id);
            SelectedVehicle = chosen is null ? null : Vehicles.First(v => v.Id == chosen.Id);
            Apply(chosen);
        }
        catch (Exception ex)
        {
            HeaderText = "Araçlar yüklenemedi";
            DetailText = ex.Message;
        }
        finally
        {
            _isLoading = false;
        }
    }

    partial void OnSelectedVehicleChanged(VehicleItem? value)
    {
        if (_isLoading || value is null) return;
        Apply(value.Vehicle);
    }

    private void Apply(Vehicle? vehicle)
    {
        _selection.Current = vehicle;

        if (vehicle is null)
        {
            HeaderText = "Araç seçilmedi";
            DetailText = "Araçlar sayfasından ilk aracını ekle.";
            return;
        }

        HeaderText = $"{vehicle.Brand} {vehicle.Model} · {VehiclePlate.Format(vehicle.Plate)}";
        var fuel = FuelLabel(vehicle.PrimaryFuelType) + (vehicle.HasLpg ? " + LPG" : string.Empty);
        DetailText = $"{vehicle.Year} · {vehicle.CurrentOdometer:N0} km · {fuel}";
    }

    private static string FuelLabel(FuelType type) => type switch
    {
        FuelType.Petrol => "Benzin",
        FuelType.Diesel => "Dizel",
        FuelType.Lpg => "LPG",
        FuelType.Hybrid => "Hibrit",
        FuelType.Electric => "Elektrik",
        _ => string.Empty
    };
}
