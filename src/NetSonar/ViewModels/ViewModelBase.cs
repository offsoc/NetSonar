using System;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using NetSonar.Avalonia.Extensions;
using NetSonar.Avalonia.Settings;

namespace NetSonar.Avalonia.ViewModels;

public partial class ViewModelBase : ObservableValidatorExtended
{
    public RuntimeGlobals RuntimeGlobals => App.RuntimeGlobals;
    public AppSettings AppSettings => App.AppSettings;

    public TopLevel TopLevel => App.TopLevel;

    public SukiDialogManager DialogManager => App.DialogManager;
    public SukiToastManager ToastManager => App.ToastManager;

    /*public static TopLevel? TopLevel
    {
        get
        {
            if (Application.Current is null) return null;
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow;
            }

            return null;
        }
    }*/

    /// <summary>
    /// Called when the control finishes initialization.
    /// </summary>
    protected internal virtual void OnInitialized() { }

    /// <summary>
    /// Called when the control finishes load.
    /// </summary>
    protected internal virtual void OnLoaded(){}

    /// <summary>
    /// Called when the control finishes unload.
    /// </summary>
    protected internal virtual void OnUnloaded(){}

    [RelayCommand]
    public Task<bool> LaunchUriAsync(string url)
    {
        return App.LaunchUriAsync(url);
    }

    [RelayCommand]
    public Task CopyToClipboardWithToast(object? text)
    {
        return App.CopyToClipboard(text?.ToString(), true);
    }

    [RelayCommand]
    public Task CopyToClipboardWithoutToast(object? text)
    {
        return App.CopyToClipboard(text?.ToString(), false);
    }


    public SukiDialogBuilder CreateMessageBox(NotificationType type, string? title = null, object? content = null)
    {
        return CreateMessageBox(title, content).OfType(type);
    }

    public SukiDialogBuilder CreateMessageBox(string? title = null, object? content = null)
    {
        var dialog = DialogManager.CreateDialog();
        if (title is not null)
        {
            dialog.SetTitle(title);
        }
        if (content is not null)
        {
            dialog.SetContent(content);
        }

        return dialog;
    }

    public SukiDialogBuilder CreateMessageBoxYesNo(NotificationType type, string? title, object? content,
        Action<ISukiDialog> yesAction, Action<ISukiDialog>? noAction = null)
    {
        var dialog = CreateMessageBox(type, title, content)
            .WithActionButton("Yes", yesAction, true)
            .WithActionButton("No", noAction ?? (_ => { }), true);


        if (dialog.Dialog.ActionButtons[0] is Button yesButton)
        {
            yesButton.IsDefault = true;
            yesButton.HotKey = new KeyGesture(Key.Enter);
        }
        if (dialog.Dialog.ActionButtons[1] is Button noButton)
        {
            noButton.IsCancel = true;
        }

        return dialog;
    }

    public SukiDialogBuilder CreateMessageBoxYesNoCancel(NotificationType type, string? title, object? content,
        Action<ISukiDialog> yesAction, Action<ISukiDialog>? noAction = null, Action<ISukiDialog>? cancelAction = null)
    {

        var dialog = CreateMessageBox(type, title, content)
            .WithActionButton("Yes", yesAction, true)
            .WithActionButton("No", noAction ?? (_ => { }), true)
            .WithActionButton("Cancel", cancelAction ?? (_ => { }), true);

        if (dialog.Dialog.ActionButtons[0] is Button yesButton)
        {
            yesButton.IsDefault = true;
            yesButton.HotKey = KeyGesture.Parse("Enter");
        }
        if (dialog.Dialog.ActionButtons[2] is Button cancelButton)
        {
            cancelButton.IsCancel = true;
        }

        return dialog;
    }
}
