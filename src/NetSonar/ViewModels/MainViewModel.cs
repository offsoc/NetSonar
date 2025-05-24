using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSonar.Avalonia.SystemOS;
using NetSonar.Avalonia.ViewModels.Dialogs;
using SukiUI.Dialogs;
using ZLinq;

namespace NetSonar.Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public IReadOnlyList<PageViewModelBase> Pages { get; init; }

    [ObservableProperty]
    public partial PageViewModelBase ActivePage { get; set; } = null!;

    public MainViewModel(IEnumerable<PageViewModelBase> pages)
    {
        Pages = new List<PageViewModelBase>(pages);
        if (Pages.Count > 0) ActivePage = Pages[0];
    }

    [RelayCommand]
    public void ShowSettingsCommand()
    {
        var settings = Pages
            .AsValueEnumerable()
            .FirstOrDefault(page => page is SettingsPageModel);
        if (settings is null) return;
        ActivePage = settings;
    }

    [RelayCommand]
    public void ShowAboutDialog()
    {
        var dialog = DialogManager
            .CreateDialog()
            .WithViewModel(dialog => new AboutDialogModel(dialog));
        dialog.TryShow();
    }

    [RelayCommand]
    public void OpenProfileFolder()
    {
        SystemAware.StartProcess(App.ProfilePath);
    }

    [RelayCommand]
    public Task CheckForUpdatesAsync()
    {
        return App.CheckForUpdatesAsync();
    }

    [RelayCommand]
    public void OpenExecutableFolder()
    {
        SystemAware.StartProcess(AppContext.BaseDirectory);
    }


    [RelayCommand]
    public void ThrowException()
    {
        int i = 50;
        int zero = 0;

        // ReSharper disable once IntDivisionByZero
        var result = i / zero;
    }
}
