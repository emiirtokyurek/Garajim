using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using Garajim.App.Services;
using Garajim.App.ViewModels;
using Garajim.Core.Abstractions;
using Garajim.Core.Services;
using Garajim.Data;
using Garajim.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Garajim.App;

public partial class App : Application
{
    private ServiceProvider? _services;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Sayı, para ve tarih biçimleri Türkçe olsun (ondalık virgül, binlik nokta).
        var culture = new CultureInfo("tr-TR");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(culture.IetfLanguageTag)));

        var services = new ServiceCollection();
        services.AddDbContextFactory<GarajimDbContext>(options =>
            options.UseSqlite($"Data Source={DatabasePaths.DefaultFilePath}"));

        services.AddSingleton<IVehicleRepository, VehicleRepository>();
        services.AddSingleton<VehicleService>();
        services.AddSingleton<VehicleSelection>();

        services.AddSingleton<DashboardViewModel>();
        services.AddSingleton<VehiclesViewModel>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();
        _services = services.BuildServiceProvider();

        try
        {
            // Veritabanı yoksa oluşturur, eksik migration varsa uygular.
            using var db = _services.GetRequiredService<IDbContextFactory<GarajimDbContext>>().CreateDbContext();
            db.Database.Migrate();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Veritabanı hazırlanamadı:\n\n{ex.Message}", "Garajım",
                MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
            return;
        }

        _services.GetRequiredService<MainWindow>().Show();

        // Açılışta ana sayfanın araç listesini yükle.
        _ = _services.GetRequiredService<DashboardViewModel>().LoadAsync();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _services?.Dispose();
        base.OnExit(e);
    }
}
