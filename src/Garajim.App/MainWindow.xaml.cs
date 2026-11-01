using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Garajim.App.ViewModels;

namespace Garajim.App;

public partial class MainWindow : Window
{
    private const int DwmUseImmersiveDarkMode = 20;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        SourceInitialized += (_, _) => EnableDarkTitleBar();
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

    /// <summary>Windows 10/11'de başlık çubuğunu da koyu yapar. Desteklenmeyen sürümde sessizce atlanır.</summary>
    private void EnableDarkTitleBar()
    {
        var handle = new WindowInteropHelper(this).Handle;
        var enabled = 1;
        DwmSetWindowAttribute(handle, DwmUseImmersiveDarkMode, ref enabled, sizeof(int));
    }
}
