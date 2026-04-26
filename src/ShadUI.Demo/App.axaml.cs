using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ShadUI.Demo.Services;
using System.Threading;

namespace ShadUI.Demo;

public class App : Application
{
    private static Mutex? _appMutex;
    private ServiceProvider? _provider;
    private ChartFontProvider? _chartFontProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            base.OnFrameworkInitializationCompleted();
            return;
        }

        _appMutex = new Mutex(true, "ShadUISingleInstanceMutex", out var createdNew);
        if (!createdNew)
        {
            new InstanceDialog().Show();
            base.OnFrameworkInitializationCompleted();
            return;
        }


        _provider = new ServiceProvider().RegisterDialogs();

        var themeWatcher = _provider.GetService<ThemeWatcher>();
        themeWatcher.Initialize();

        _chartFontProvider = _provider.GetService<ChartFontProvider>();

        var viewModel = _provider.GetService<MainWindowViewModel>();
        viewModel.Initialize();

        var mainWindow = new MainWindow { DataContext = viewModel };
        this.RegisterTrayIconsEvents(mainWindow, viewModel);

        desktop.MainWindow = mainWindow;

        desktop.Exit += OnExit;

        base.OnFrameworkInitializationCompleted();
    }

    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        _chartFontProvider?.Dispose();
    }
}
