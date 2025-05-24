using System.Diagnostics.CodeAnalysis;
using System;
using System.IO;

namespace NetSonar.Avalonia;

public partial class App
{
    [field: AllowNull, MaybeNull]
    public static string ProfilePath
    {
        get
        {
            if (field is null)
            {
                if (OperatingSystem.IsWindows())
                {
                    field = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        Software);
                }
                else if (OperatingSystem.IsLinux())
                {
                    field = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        Software);
                }
                else if (OperatingSystem.IsMacOS())
                {
                    field = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                        "Library",
                        "Application Support",
                        Software);
                }
                else
                {
                    field = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Software);
                }
            }

            return field;
        }
    }

    public static string LogsPath => Path.Combine(ProfilePath, "logs");
    public static string LogFile => Path.Combine(LogsPath, "app.log");
    public static string CrashReportsFile => Path.Combine(LogsPath, "crash_reports.json");

    public static string BackupsPath => Path.Combine(ProfilePath, "backups");

    public static string ConfigPath => Path.Combine(ProfilePath, "settings");
    public static string SettingsFile => Path.Combine(ConfigPath, "app_settings.json");
}