using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garajim.Core.Entities;
using Garajim.Core.Enums;
using Garajim.Core.Services;
using Garajim.Core.Validation;

namespace Garajim.App.ViewModels;

/// <summary>Yakıt türü seçim çiplerinde gösterilen öğe.</summary>
public sealed record FuelOption(FuelType Type, string Label);

/// <summary>Listedeki bir araç satırı.</summary>
public sealed class VehicleItem
{
    public VehicleItem(Vehicle vehicle) => Vehicle = vehicle;

    public Vehicle Vehicle { get; }
    public int Id => Vehicle.Id;
    public string Title => $"{Vehicle.Brand} {Vehicle.Model}";
    public string PlateText => VehiclePlate.Format(Vehicle.Plate);
    public string Subtitle => $"{Vehicle.Year} · {Vehicle.CurrentOdometer:N0} km";
    public bool IsArchived => Vehicle.IsArchived;
}

public partial class VehiclesViewModel : ObservableObject
{
    private readonly VehicleService _service;
    private int _editingId; // 0 = yeni araç formu

    public VehiclesViewModel(VehicleService service)
    {
        _service = service;
        FuelOptions = new[]
        {
            new FuelOption(FuelType.Petrol, "Benzin"),
            new FuelOption(FuelType.Diesel, "Dizel"),
            new FuelOption(FuelType.Hybrid, "Hibrit"),
            new FuelOption(FuelType.Electric, "Elektrik")
        };
        StartNew();
    }

    public ObservableCollection<VehicleItem> Vehicles { get; } = new();
    public IReadOnlyList<FuelOption> FuelOptions { get; }

    [ObservableProperty] private VehicleItem? _selectedItem;
    [ObservableProperty] private bool _showArchived;

    [ObservableProperty] private string _formTitle = "Yeni araç";
    [ObservableProperty] private string _brand = string.Empty;
    [ObservableProperty] private string _model = string.Empty;
    [ObservableProperty] private string _plate = string.Empty;
    [ObservableProperty] private string _engineInfo = string.Empty;
    [ObservableProperty] private string _yearText = string.Empty;
    [ObservableProperty] private string _odometerText = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLpgAvailable))]
    private FuelOption? _selectedFuel;

    [ObservableProperty] private bool _hasLpg;

    [ObservableProperty] private bool _isEditing;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ArchiveButtonText))]
    private bool _isCurrentArchived;

    [ObservableProperty] private string _errorText = string.Empty;
    [ObservableProperty] private string _statusText = string.Empty;

    public bool IsLpgAvailable => SelectedFuel?.Type == FuelType.Petrol;
    public string ArchiveButtonText => IsCurrentArchived ? "Arşivden çıkar" : "Arşivle";

    partial void OnSelectedItemChanged(VehicleItem? value)
    {
        // Liste yenilenirken seçim geçici olarak null olur; formu bozmamak için yok sayılır.
        if (value is not null) FillForm(value.Vehicle);
    }

    partial void OnSelectedFuelChanged(FuelOption? value)
    {
        if (value?.Type != FuelType.Petrol) HasLpg = false;
    }

    partial void OnShowArchivedChanged(bool value) => _ = LoadAsync();

    public async Task LoadAsync()
    {
        try
        {
            var list = ShowArchived ? await _service.GetAllAsync() : await _service.GetActiveAsync();
            var keepId = _editingId;

            Vehicles.Clear();
            foreach (var vehicle in list) Vehicles.Add(new VehicleItem(vehicle));

            SelectedItem = Vehicles.FirstOrDefault(v => v.Id == keepId);
        }
        catch (Exception ex)
        {
            ErrorText = $"Liste yüklenemedi: {ex.Message}";
        }
    }

    [RelayCommand]
    private void NewVehicle() => StartNew();

    [RelayCommand]
    private async Task SaveAsync()
    {
        ErrorText = string.Empty;
        StatusText = string.Empty;

        if (!TryParseNumber(YearText, out var year)) { ErrorText = "Model yılı sayı olmalı."; return; }
        if (!TryParseNumber(OdometerText, out var odometer)) { ErrorText = "Kilometre sayı olmalı."; return; }

        var vehicle = new Vehicle
        {
            Id = _editingId,
            Brand = Brand,
            Model = Model,
            Year = year,
            EngineInfo = EngineInfo,
            Plate = Plate,
            CurrentOdometer = odometer,
            PrimaryFuelType = SelectedFuel?.Type ?? default,
            HasLpg = HasLpg
        };

        try
        {
            var result = _editingId == 0
                ? await _service.AddAsync(vehicle)
                : await _service.UpdateAsync(vehicle);

            if (!result.IsValid)
            {
                ErrorText = string.Join(Environment.NewLine, result.Errors);
                return;
            }

            _editingId = vehicle.Id; // yeni eklenen araçta Id artık dolu
            await LoadAsync();
            StatusText = "Kaydedildi.";
        }
        catch (Exception ex)
        {
            ErrorText = $"Kaydedilemedi: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ToggleArchiveAsync()
    {
        if (_editingId == 0) return;

        try
        {
            var archiving = !IsCurrentArchived;
            if (archiving) await _service.ArchiveAsync(_editingId);
            else await _service.RestoreAsync(_editingId);

            // Arşivlenen araç listeden kayboluyorsa formu da temizle.
            if (archiving && !ShowArchived) StartNew();

            await LoadAsync();
            StatusText = archiving ? "Araç arşive alındı." : "Araç arşivden çıkarıldı.";
        }
        catch (Exception ex)
        {
            ErrorText = $"İşlem yapılamadı: {ex.Message}";
        }
    }

    private void StartNew()
    {
        _editingId = 0;
        IsEditing = false;
        IsCurrentArchived = false;
        FormTitle = "Yeni araç";
        Brand = Model = Plate = EngineInfo = YearText = OdometerText = string.Empty;
        SelectedFuel = FuelOptions[0];
        HasLpg = false;
        ErrorText = string.Empty;
        StatusText = string.Empty;
        SelectedItem = null;
    }

    private void FillForm(Vehicle vehicle)
    {
        _editingId = vehicle.Id;
        IsEditing = true;
        IsCurrentArchived = vehicle.IsArchived;
        FormTitle = $"{vehicle.Brand} {vehicle.Model} · düzenle";
        Brand = vehicle.Brand;
        Model = vehicle.Model;
        Plate = VehiclePlate.Format(vehicle.Plate);
        EngineInfo = vehicle.EngineInfo ?? string.Empty;
        YearText = vehicle.Year.ToString(CultureInfo.InvariantCulture);
        OdometerText = vehicle.CurrentOdometer.ToString("N0");
        SelectedFuel = FuelOptions.FirstOrDefault(f => f.Type == vehicle.PrimaryFuelType);
        HasLpg = vehicle.HasLpg; // yakıt seçiminden SONRA atanır, yoksa sıfırlanır
        ErrorText = string.Empty;
    }

    /// <summary>"90.000", "90 000" ve "90000" hepsi kabul edilir.</summary>
    private static bool TryParseNumber(string? text, out int value)
    {
        var digits = Regex.Replace(text ?? string.Empty, @"[\s\.,]", string.Empty);
        return int.TryParse(digits, NumberStyles.None, CultureInfo.InvariantCulture, out value);
    }
}
