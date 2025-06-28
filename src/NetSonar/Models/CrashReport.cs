using Avalonia;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Threading;
using Updatum;
using Utf8StringInterpolation;

namespace NetSonar.Avalonia.Models;

public sealed record CrashReport
{
    /// <summary>
    /// Gets or sets the ID.
    /// </summary>
    public long Id { get; init; } = Stopwatch.GetTimestamp();

    /// <summary>
    /// Gets or sets the software version.
    /// </summary>
    public string Version { get; init; } = EntryApplication.AssemblyVersionString ?? string.Empty;

    /// <summary>
    /// Gets or sets the exception.
    /// </summary>
    public required ExceptionInfo Exception { get; init; }

    /// <summary>
    /// Gets or sets the category.
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// Gets or sets the date and time when the crash occurred in UTC time.
    /// </summary>
    public required DateTime DateTimeUtc { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the date and time when the crash occurred in local time.
    /// </summary>
    public DateTime DateTime => DateTimeUtc.ToLocalTime();

    /// <summary>
    /// Gets the managed thread count when the crash occurred.
    /// </summary>
    public int ThreadPoolCount { get; init; }

    /// <summary>
    /// Gets the total thread count when the crash occurred.
    /// </summary>
    public int ThreadCount { get; init; }

    /// <summary>
    /// Gets the system CPU usage time when the crash occurred.
    /// </summary>
    public TimeSpan CpuPrivilegedTime { get; init; }

    /// <summary>
    /// Gets the system CPU usage time when the crash occurred.
    /// </summary>
    public TimeSpan CpuUserTime { get; init; }

    /// <summary>
    /// Gets the total CPU usage time when the crash occurred.
    /// </summary>
    public TimeSpan CpuTotalTime => CpuPrivilegedTime + CpuUserTime;

    /// <summary>
    /// Gets the amount of physical memory, in bytes, allocated for the process when the crash occurred.
    /// </summary>
    public long ProcessWorkingSet64 { get; init; }

    [JsonIgnore]
    public string FormatedMessage
    {
        get
        {
            using var writer = Utf8String.CreateWriter(out var zsb);

            var exception = Exception;
            var i = 1;
            do
            {
                /*var advance = new string('-', i*2);
                if (advance.Length > 0)
                {
                    advance = $"{advance} ";
                }*/

                if ((i == 1 && exception.InnerException is not null) || i > 1)
                {
                    zsb.AppendLine($"## {i} ##");
                }
                zsb.AppendLine($"{exception.Message}");
                zsb.AppendLine($"{exception.Type}");
                zsb.AppendLine($"{exception.StackTrace}");
                exception = exception.InnerException;
                if (exception is not null)
                {
                    zsb.AppendLine();
                }

                i++;
            } while (exception is not null);

            zsb.AppendLine();
            zsb.AppendLine($"ID: {Id}");
            zsb.AppendLine($"Version: {Version} {RuntimeInformation.ProcessArchitecture}");
            zsb.AppendLine($"Category: {Category}");
            zsb.AppendLine($"UTC date time: {DateTimeUtc}");
            zsb.AppendLine($"Machine date time: {DateTime}");
            zsb.AppendLine($"Operative System: {RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture}");
            zsb.AppendLine($"Framework: {RuntimeInformation.FrameworkDescription}   UI: Avalonia {typeof(AvaloniaObject).Assembly.GetName().Version!.ToString(3)}   Runtime: {RuntimeInformation.RuntimeIdentifier}");
            zsb.AppendLine($"Processor cores: {Environment.ProcessorCount}   Thread usage: {ThreadPoolCount} / {ThreadCount}");
            zsb.AppendLine($"CPU usage: System={CpuPrivilegedTime}   User={CpuUserTime}   Total={CpuTotalTime}");
            zsb.AppendLine($"RAM usage: {CommunityToolkit.Common.Converters.ToFileSizeString(ProcessWorkingSet64)}");
            zsb.Flush();
            return writer.ToString();
        }
    }

    public CrashReport()
    {
    }

    [SetsRequiredMembers]
    public CrashReport(Exception exception, string category, DateTime dateTimeUtc)
    {
        Exception = new  ExceptionInfo(exception);
        Category = category;
        DateTimeUtc = dateTimeUtc;

        using var process = Process.GetCurrentProcess();

        ThreadPoolCount = ThreadPool.ThreadCount;
        ThreadCount = process.Threads.Count;
        CpuPrivilegedTime = process.PrivilegedProcessorTime;
        CpuUserTime = process.UserProcessorTime;
        ProcessWorkingSet64 = process.WorkingSet64;
    }

    [SetsRequiredMembers]
    public CrashReport(Exception exception, string category) : this(exception, category, DateTime.UtcNow)
    {
    }
}