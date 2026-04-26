using System;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ShadUI.Demo.Services;
using SkiaSharp;

namespace ShadUI.Demo.ViewModels;

[Page("dashboard")]
public sealed partial class DashboardViewModel : ViewModelBase, INavigable
{
    private readonly double[] _chartValues = new double[12];
    private readonly SolidColorPaint _seriesFillPaint;
    private readonly SolidColorPaint _axisLabelPaint;

    [ObservableProperty]
    private SolidColorPaint _tooltipTextPaint = null!;

    private readonly PageManager _pageManager;
    private readonly EventHandler<ThemeColors> _themeChangedHandler;
    private readonly ChartFontProvider _chartFontProvider;
    public ThemeWatcher ThemeWatcher { get; }

    public DashboardViewModel(
        PageManager pageManager,
        ThemeWatcher themeWatcher,
        ChartFontProvider chartFontProvider
    )
    {
        _pageManager = pageManager;
        ThemeWatcher = themeWatcher;
        _chartFontProvider = chartFontProvider;
        _themeChangedHandler = OnThemeColorsChanged;
        ThemeWatcher.ThemeChanged += _themeChangedHandler;

        var initialForeground = ToSKColor(ThemeWatcher.ThemeColors.ForegroundColor);

        _axisLabelPaint = new SolidColorPaint(initialForeground) { SKTypeface = _chartFontProvider.Typeface };
        _tooltipTextPaint = new SolidColorPaint(initialForeground) { SKTypeface = _chartFontProvider.Typeface };
        _seriesFillPaint = new SolidColorPaint(SKColors.Transparent);

        FillChartValues(_chartValues);

        SeriesWide = [new ColumnSeries<double> { Values = _chartValues, Fill = _seriesFillPaint }];
        SeriesCompact =
        [
            new ColumnSeries<double> { Values = _chartValues, Fill = _seriesFillPaint },
        ];

        XAxes =
        [
            new Axis
            {
                Labels =
                [
                    "Jan",
                    "Feb",
                    "Mar",
                    "Apr",
                    "May",
                    "Jun",
                    "Jul",
                    "Aug",
                    "Sep",
                    "Oct",
                    "Nov",
                    "Dec",
                ],
                LabelsPaint = _axisLabelPaint,
                TextSize = 12,
                MinStep = 1,
            },
        ];

        YAxes =
        [
            new Axis
            {
                Labeler = Labelers.Currency,
                LabelsPaint = _axisLabelPaint,
                TextSize = 12,
                MinStep = 1500,
                ShowSeparatorLines = false,
            },
        ];
    }

    public ISeries[] SeriesWide { get; }
    public ISeries[] SeriesCompact { get; }
    public Axis[] XAxes { get; set; }
    public Axis[] YAxes { get; set; }

    public void Initialize()
    {
        FillChartValues(_chartValues);
        ((ColumnSeries<double>)SeriesWide[0]).Values = _chartValues;
        ((ColumnSeries<double>)SeriesCompact[0]).Values = _chartValues;
        UpdatePaints(ThemeWatcher.ThemeColors);
    }

    private void OnThemeColorsChanged(object? sender, ThemeColors colors) => UpdatePaints(colors);

    private void UpdatePaints(ThemeColors colors)
    {
        var foreground = ToSKColor(colors.ForegroundColor);
        _axisLabelPaint.Color = foreground;
        _axisLabelPaint.SKTypeface = _chartFontProvider.Typeface;
        TooltipTextPaint.Color = foreground;
        TooltipTextPaint.SKTypeface = _chartFontProvider.Typeface;
        _seriesFillPaint.Color = ToSKColor(colors.PrimaryColor);
    }

    private static void FillChartValues(double[] values)
    {
        var random = new Random();
        for (var i = 0; i < values.Length; i++)
            values[i] = random.Next(1000, 5000);
    }

    private static SKColor ToSKColor(Color color) => new(color.R, color.G, color.B, color.A);

    [RelayCommand]
    private void NextPage() => _pageManager.Navigate<ThemeViewModel>();

    public override void Dispose()
    {
        ThemeWatcher.ThemeChanged -= _themeChangedHandler;
        base.Dispose();
    }
}
