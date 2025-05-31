using System.ComponentModel;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using ObservableCollections;
using SukiUI.Models;

namespace NetSonar.Avalonia.ViewModels;

public partial class SettingsPageModel : PageViewModelBase
{
    public override int Index => -1;
    public override string DisplayName => "Settings";
    public override MaterialIconKind Icon => MaterialIconKind.Cog;
    public override bool AutoHideOnSideMenu => true;

    [ObservableProperty]
    public partial bool IsSystemTheme { get; set; }

    [ObservableProperty]
    public partial bool IsLightTheme { get; set; }

    [ObservableProperty]
    public partial bool IsDarkTheme { get; set; }

    public ObservableList<SukiColorTheme> AvailableColors { get; } = [.. App.Theme.ColorThemes];


    public SettingsPageModel()
    {
        IsVisibleOnSideMenu = false;

        IsSystemTheme = AppSettings.Theme == ApplicationTheme.Default;
        IsLightTheme = AppSettings.Theme == ApplicationTheme.Light;
        IsDarkTheme = AppSettings.Theme == ApplicationTheme.Dark;


        AvailableColors.AddRange([
            new SukiColorTheme("Pink", new Color(255, 255, 20, 147), new Color(255, 255, 192, 203)),
            new SukiColorTheme("White", new Color(255, 255, 255, 255), new Color(255, 0, 0, 0)),
            new SukiColorTheme("Black", new Color(255, 0, 0, 0), new Color(255, 255, 255, 255))
            ]);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsSystemTheme))
        {
            if (IsSystemTheme) App.ChangeBaseTheme(ApplicationTheme.Default);
        }
        else if (e.PropertyName == nameof(IsLightTheme))
        {
            if (IsLightTheme) App.ChangeBaseTheme(ApplicationTheme.Light);
        }
        else if (e.PropertyName == nameof(IsDarkTheme))
        {
            if (IsDarkTheme) App.ChangeBaseTheme(ApplicationTheme.Dark);
        }
        base.OnPropertyChanged(e);
    }

    [RelayCommand]
    public static void SwitchToColorThemeCommand(SukiColorTheme color)
    {
        AppSettings.ThemeColor = color.DisplayName;
        App.Theme.ChangeColorTheme(color);
    }
}