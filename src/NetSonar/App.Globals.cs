using NetSonar.Avalonia.Converters;
using NetSonar.Avalonia.Settings;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace NetSonar.Avalonia;

public partial class App
{
    public static readonly RuntimeGlobals RuntimeGlobals = new();
    public static AppSettings AppSettings => AppSettings.Instance;

    public static readonly HttpClient HttpClient = new();

    public static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        IgnoreReadOnlyFields = true,
        //IgnoreReadOnlyProperties = true,

        Converters =
        {
            new IPAddressJsonConverter(),
            new IPEndPointJsonConverter(),
            new JsonStringEnumConverter(),
        }
    };
}