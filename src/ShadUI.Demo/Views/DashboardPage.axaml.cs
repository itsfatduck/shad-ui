using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;
using ShadUI.Demo.ViewModels;

namespace ShadUI.Demo.Views;

public partial class DashboardPage : UserControl
{
    public DashboardPage()
    {
        InitializeComponent();
        // LiveCharts / theme refresh — restore when re-enabling CartesianChart in DashboardPage.axaml
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DashboardViewModel vm) return;
        vm.ThemeWatcher.ThemeChanged -= OnThemeChanged;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DashboardViewModel vm) return;
        vm.ThemeWatcher.ThemeChanged += OnThemeChanged;
    }

    private void OnThemeChanged(object? sender, ThemeColors e)
    {

        // TODO: Re-enable when LiveCharts is restored
        /*
        Dispatcher.UIThread.Post(() =>
        {
            CartesianChart1.CoreChart.Update(new ChartUpdateParams
                { IsAutomaticUpdate = false, Throttling = false });
            CartesianChart2.CoreChart.Update(new ChartUpdateParams
                { IsAutomaticUpdate = false, Throttling = false });
        });
        */
        // Invalidate BitmapCache for all Border elements when theme changes
        // This ensures shadows and backgrounds render with correct theme colors
        InvalidateBitmapCaches(this);
    }

    private static void InvalidateBitmapCaches(Visual visual)
    {
        foreach (var child in visual.GetVisualChildren())
        {
            if (child is Border border && border.CacheMode is not null)
            {
                // Clear and restore cache to force re-render
                var cache = border.CacheMode;
                border.CacheMode = null;
                border.CacheMode = cache;
            }
            InvalidateBitmapCaches(child);
        }
    }

    
}
