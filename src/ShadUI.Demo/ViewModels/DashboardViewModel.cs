using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShadUI.Demo.ViewModels;

[Page("dashboard")]
public sealed partial class DashboardViewModel : ViewModelBase, INavigable
{
    // TODO: Pending Avalonia 12 support - re-enable dashboard chart integration.
    private readonly PageManager _pageManager;

    public DashboardViewModel(PageManager pageManager, ThemeWatcher themeWatcher)
    {
        _pageManager = pageManager;
        _ = themeWatcher;
        // TODO: Pending Avalonia 12 support - restore chart data/series setup when chart library is compatible.
    }

    [RelayCommand]
    private void NextPage()
    {
        _pageManager.Navigate<ThemeViewModel>();
    }

    public void Initialize()
    {
        // TODO: Pending Avalonia 12 support - re-enable chart initialization.
    }
}