using Avalonia;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Platform.Storage;

namespace NetSonar.Avalonia.Extensions;

public static class AvaloniaExtensions
{
    public static IEnumerable<T> FindChildren<T>(this Visual parent)
    {
        foreach (var child in parent.GetVisualChildren())
        {
            if (child is T target)
                yield return target;

            foreach (var item in FindChildren<T>(child))
                yield return item;
        }
    }

    public static void ExtendDataGridShortcuts(this DataGrid dataGrid)
    {
        void OnDataGridOnKeyUp(object? sender, KeyEventArgs e)
        {
            if (e.KeyModifiers == KeyModifiers.Shift)
            {
                if (e.Key == Key.Escape)
                {
                    dataGrid.SelectedIndex = -1;
                    e.Handled = true;
                    return;
                }

                if (e.Key == Key.Multiply)
                {
                    var invertedList = new List<object?>();
                    foreach (var item in dataGrid.ItemsSource)
                    {
                        if (!dataGrid.SelectedItems.Contains(item))
                        {
                            invertedList.Add(item);
                        }
                    }

                    dataGrid.SelectedItems.Clear();
                    foreach (var host in invertedList)
                    {
                        dataGrid.SelectedItems.Add(host);
                    }

                    e.Handled = true;
                    return;
                }
            }
        }

        dataGrid.KeyUp += OnDataGridOnKeyUp;
    }

    public static FilePickerFileType[] FilePickerJson { get; } = [
        new("JSON files")
        {
            Patterns = ["*.json"],
            AppleUniformTypeIdentifiers = ["public.plain-text"],
            MimeTypes = ["text/json"]
        }
    ];

    public static FilePickerFileType[] FilePickerCsv { get; } = [
        new("CSV files")
        {
            Patterns = ["*.csv"],
            AppleUniformTypeIdentifiers = ["public.plain-text"],
            MimeTypes = ["text/csv"]
        }
    ];

    public static FilePickerFileType[] FilePickerIni { get; } = [
        new("INI files")
        {
            Patterns = ["*.ini"],
            AppleUniformTypeIdentifiers = ["public.plain-text"],
            MimeTypes = ["text/ini"]
        }
    ];

}