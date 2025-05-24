using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using Avalonia.Controls;
using ZLinq;

namespace NetSonar.Avalonia.ViewModels.Dialogs;

public partial class InstanceAlreadyRunningDialogModel : ViewModelBase
{
    public string Message { get; init; }
    public Process? FirstProcess { get; init; }

    public InstanceAlreadyRunningDialogModel()
    {
        var processes = Process.GetProcessesByName(App.Software);

        Message = $"""
                   There is another instance of {App.Software} running. Only one instance of {App.Software} can run at a time.
                   Please find and open the running instance or close it before starting a new one.
                   """;

        if (Design.IsDesignMode)
        {
            Message += "\n\nProcess ID: 1001";
        }
        else
        {
            if (processes.Length > 1)
            {
                FirstProcess = processes
                    .AsValueEnumerable()
                    .FirstOrDefault(p => p.Id != Environment.ProcessId);
                if (FirstProcess is not null)
                {
                    Message += $"\n\nProcess ID: {FirstProcess.Id}";
                }
            }
        }
    }

    [RelayCommand]
    public void CloseWindow()
    {
        Environment.Exit(0);
    }
}