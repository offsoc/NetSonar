using Avalonia.Controls;
using NetSonar.Avalonia.Controls;
using NetSonar.Avalonia.Extensions;

namespace NetSonar.Avalonia.Views.Dialogs;

public partial class AddPingServicesDialogView : UserControlBase
{
    public AddPingServicesDialogView()
    {
        InitializeComponent();
        ServicesGrid.ExtendDataGridShortcuts();
    }
}