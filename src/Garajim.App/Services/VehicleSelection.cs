using CommunityToolkit.Mvvm.ComponentModel;
using Garajim.Core.Entities;

namespace Garajim.App.Services;

/// <summary>Uygulama genelinde "şu an hangi araçla çalışıyoruz" bilgisi. Yakıt, bakım vb. sayfalar bunu okur.</summary>
public sealed partial class VehicleSelection : ObservableObject
{
    [ObservableProperty]
    private Vehicle? _current;
}
