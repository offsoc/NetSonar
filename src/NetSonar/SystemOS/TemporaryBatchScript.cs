using Cysharp.Diagnostics;
using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Utf8StringInterpolation;

namespace NetSonar.Avalonia.SystemOS;

public class TemporaryBatchScript : IDisposable
{
    private readonly StringBuilder _builder = new StringBuilder();

    /// <summary>
    /// Gets or sets a value indicating whether the batch script requires admin rights.
    /// </summary>
    public bool RequireAdminRights { get; set; }

    /// <summary>
    /// Gets the path to the temporary batch script file.
    /// </summary>
    public string FilePath { get; }

    public TemporaryBatchScript(bool requireAdminRights = false)
    {
        RequireAdminRights = requireAdminRights;

        FilePath = Path.GetTempFileName();
        if (OperatingSystem.IsWindows())
        {
            FilePath += ".bat";
        }
        else
        {
            FilePath += ".sh";
        }

        if (OperatingSystem.IsWindows())
        {
            _builder.AppendLine("@echo off");
        }
    }

    /// <summary>
    /// Writes the specified text to the batch script.
    /// </summary>
    /// <param name="text"></param>
    public void Write(string text)
    {
        _builder.Append(text);
    }

    /// <summary>
    /// Writes a newline character to the batch script.
    /// </summary>
    public void WriteLine()
    {
        _builder.AppendLine();
    }

    /// <summary>
    /// Writes the specified text to the batch script followed by a newline character.
    /// </summary>
    /// <param name="text"></param>
    public void WriteLine(string text)
    {
        _builder.AppendLine(text);
    }

    /// <summary>
    /// Executes the batch script and returns the output.
    /// </summary>
    /// <returns></returns>
    public async Task<string[]> Execute()
    {
        string process;

        if (OperatingSystem.IsWindows())
        {
            process = $"cmd /c \"{FilePath}\"";
            if (RequireAdminRights) process = $"gsudo {process}";
        }
        else
        {
            process = $"bash \"{FilePath}\"";
        }

        try
        {
            await File.WriteAllTextAsync(FilePath, _builder.ToString());
            return await ProcessX.StartAsync(process).ToTask();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            try
            {
                if (File.Exists(FilePath)) File.Delete(FilePath);
            }
            catch (Exception e)
            {
                App.WriteLine(e);
            }
        }

    }

    public void Dispose()
    {
        try
        {
            if (File.Exists(FilePath)) File.Delete(FilePath);
        }
        catch (Exception e)
        {
            App.WriteLine(e);
        }
    }
}