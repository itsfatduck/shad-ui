using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using LiveChartsCore.Kernel;
using ShadUI.Demo.ViewModels;

namespace ShadUI.Demo.Views;

public partial class DashboardPage : UserControl
{
    public DashboardPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e) => RefreshCharts();

    private void OnUnloaded(object? sender, RoutedEventArgs e) => Loaded -= OnLoaded;

    private void RefreshCharts()
    {
        Dispatcher.UIThread.Post(
            () =>
            {
                if (!IsLoaded)
                    return;
                TryUpdateChart(CartesianChart1);
                TryUpdateChart(CartesianChart2);
            },
            DispatcherPriority.Loaded
        );
    }

    private static void TryUpdateChart(LiveChartsCore.SkiaSharpView.Avalonia.CartesianChart? chart)
    {
        if (chart?.CoreChart is null)
            return;
        chart.CoreChart.Update(
            new ChartUpdateParams { IsAutomaticUpdate = false, Throttling = false }
        );
    }
}
