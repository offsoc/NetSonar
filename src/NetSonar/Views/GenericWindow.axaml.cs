using NetSonar.Avalonia.Controls;
using NetSonar.Avalonia.Settings;

namespace NetSonar.Avalonia.Views;

public partial class GenericWindow : SukiWindowExtended
{
    public static AppSettings AppSettings => AppSettings.Instance;

    public GenericWindow()
    {
        InitializeComponent();
        DataContext = this;
    }
}