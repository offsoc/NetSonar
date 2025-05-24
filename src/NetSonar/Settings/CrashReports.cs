using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using NetSonar.Avalonia.Extensions;
using NetSonar.Avalonia.Models;
using ZLinq;

namespace NetSonar.Avalonia.Settings;

public class CrashReports : List<CrashReport>
{
    #region Constants
    public const int MaxCrashReports = 50;
    #endregion

    #region Singleton
    private static readonly Lazy<CrashReports> LazyInstance = new(() =>
    {
        try
        {
            if (File.Exists(App.CrashReportsFile))
            {
                using var stream = File.OpenRead(App.CrashReportsFile);
                var crashReports = JsonSerializer.Deserialize<CrashReports>(stream, App.JsonSerializerOptions);
                if (crashReports is not null) return crashReports;
            }

        }
        catch (Exception e)
        {
            App.HandleSafeException(e, $"Read {Path.GetFileName(App.CrashReportsFile)}");
        }

        return new CrashReports();
    });

    public static CrashReports Instance => LazyInstance.Value;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets a value indicating whether the collection has changed.
    /// </summary>
    public bool IsChanged { get; set; } = false;

    #endregion

    #region Constructor
    public CrashReports()
    {
    }
    #endregion

    #region Methods

    public new void Insert(int index, CrashReport item)
    {
        base.Insert(index, item);
        IsChanged = true;
        Save();
    }

    public new void InsertRange(int index, IEnumerable<CrashReport> item)
    {
        base.InsertRange(index, item);
        IsChanged = true;
        Save();
    }

    public new void Add(CrashReport item)
    {
        base.Add(item);
        IsChanged = true;
        Save();
    }

    public new void AddRange(IEnumerable<CrashReport> items)
    {
        base.AddRange(items);
        IsChanged = true;
        Save();
    }

    public new void Clear()
    {
        base.Clear();
        IsChanged = true;
        Save();
    }

    public CrashReport? GetActual(long id)
    {
        return id == 0
            ? null
            : this.AsValueEnumerable().LastOrDefault(report => report.Id == id);
    }

    private void Save()
    {
        if (!IsChanged) return;
        this.RemoveExceedingAt(MaxCrashReports, CollectionSide.Head);
        try
        {
            Directory.CreateDirectory(App.LogsPath);
            using var stream = File.Create(App.CrashReportsFile);
            JsonSerializer.Serialize(stream, this, App.JsonSerializerOptions);
        }
        catch (Exception e)
        {
            App.HandleSafeException(e, $"Save {Path.GetFileName(App.CrashReportsFile)}");
        }
    }

    #endregion
}