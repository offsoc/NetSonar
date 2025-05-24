using Avalonia.Styling;
using SukiUI;
using System;
using ZLinq;

namespace NetSonar.Avalonia;

public partial class App
{
    public static SukiTheme Theme { get; private set; } = null!;

    private void SetupTheme()
    {
        Theme = SukiTheme.GetInstance();
        ChangeBaseTheme(AppSettings.Theme);
        if (!string.IsNullOrWhiteSpace(AppSettings.ThemeColor))
        {
            var colorTheme = Theme.ColorThemes
                .AsValueEnumerable()
                .FirstOrDefault(theme => theme.DisplayName.Equals(AppSettings.ThemeColor, StringComparison.OrdinalIgnoreCase));
            if (colorTheme is not null)
            {
                Theme.ChangeColorTheme(colorTheme);
            }
        }
    }

    public static void ChangeBaseTheme(ApplicationTheme theme)
    {
        AppSettings.Theme = theme;
        switch (theme)
        {
            case ApplicationTheme.Default:
                Theme.ChangeBaseTheme(ThemeVariant.Default);
                break;
            case ApplicationTheme.Light:
                Theme.ChangeBaseTheme(ThemeVariant.Light);
                break;
            case ApplicationTheme.Dark:
                Theme.ChangeBaseTheme(ThemeVariant.Dark);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(theme), theme, null);
        }
    }
}