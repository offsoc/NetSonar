using Avalonia;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
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
    public Version Version { get; init; } = EntryApplication.AssemblyVersion!;

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
    public required DateTime DateTimeUtc { get; init; }

    /// <summary>
    /// Gets the date and time when the crash occurred in local time.
    /// </summary>
    public DateTime DateTime => DateTimeUtc.ToLocalTime();

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
            zsb.AppendLine($"Version: {Version.ToString(3)} {RuntimeInformation.ProcessArchitecture}");
            zsb.AppendLine($"Category: {Category}");
            zsb.AppendLine($"UTC date time: {DateTimeUtc}");
            zsb.AppendLine($"Machine date time: {DateTime}");
            zsb.AppendLine($"Operative System: {RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture}");
            zsb.AppendLine($"Framework: {RuntimeInformation.FrameworkDescription}   UI: Avalonia {typeof(AvaloniaObject).Assembly.GetName().Version!.ToString(3)}");
            zsb.AppendLine($"Runtime: {RuntimeInformation.RuntimeIdentifier}");
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
    }

    [SetsRequiredMembers]
    public CrashReport(Exception exception, string category) : this(exception, category, DateTime.UtcNow)
    {
    }
}