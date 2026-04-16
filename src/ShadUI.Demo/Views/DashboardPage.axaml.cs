using Avalonia.Controls;
using Avalonia.Interactivity;
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
    }



}
