using System.Windows;
using Garajim.App.ViewModels;
using Garajim.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Garajim.App;

public partial class App : Application
{
    private ServiceProvider? _services;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        services.AddDbContextFactory<GarajimDbContext>(options =>
            options.UseSqlite($"Data Source={DatabasePaths.DefaultFilePath}"));
        services.AddSingleton<DashboardViewModel>();
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
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _services?.Dispose();
        base.OnExit(e);
    }
}
