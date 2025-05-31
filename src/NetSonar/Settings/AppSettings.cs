using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.NetworkInformation;
using System.Text.Json;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using NetSonar.Avalonia.Extensions;
using NetSonar.Avalonia.Models;
using NetSonar.Avalonia.Network;
using ObservableCollections;
using SukiUI.Enums;

namespace NetSonar.Avalonia.Settings;


public partial class PingServicesSettings : BaseSettings
{
    public enum PingServicesGroupBy
    {
        [Description("None")]
        None,

        [Description("Protocol type")]
        ProtocolType,

        [Description("Status")]
        LastStatus,

        [Description("Group")]
        Group
    }

    [ObservableProperty]
    public partial PingServicesGroupBy GridGroupBy { get; set; } = PingServicesGroupBy.Group;

    public double DefaultPingEverySeconds
    {
        get;
        set => SetProperty(ref field, Math.Clamp(value, PingableService.MinPingEverySeconds, PingableService.MaxPingEverySeconds));
    } = PingableService.DefaultPingEverySeconds;

    public double DefaultTimeoutSeconds
    {
        get;
        set => SetProperty(ref field, Math.Clamp(value, PingableService.MinTimeoutSeconds, PingableService.MaxTimeoutSeconds));
    } = PingableService.DefaultTimeoutSeconds;

    public int DefaultBufferSize
    {
        get;
        set => SetProperty(ref field, Math.Clamp(value, PingableService.MinBufferSize, PingableService.MaxBufferSize));
    } = PingableService.DefaultBufferSize;

    public byte DefaultTtl
    {
        get;
        set => SetProperty(ref field, Math.Max((byte)1, value));
    } = PingableService.DefaultTtl;

    [ObservableProperty]
    public partial bool DefaultDontFragment { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of ping replies to keep in the list.
    /// </summary>
    public int MaxRepliesCache
    {
        get;
        set => SetProperty(ref field, Math.Max(0, value));
    } = 10_000;

    /// <summary>
    /// Gets or sets the maximum number of ping replies to show in the graph.
    /// </summary>
    public int MaxRepliesGraphCache
    {
        get;
        set => SetProperty(ref field, Math.Max(0, value));
    } = 100;


    [ObservableProperty]
    public partial ObservableList<PingableService> Services { get; set; }
#if DEBUG
        = [
new (ServiceProtocolType.ICMP, "192.168.1.254", "Router", "Network"),
new (ServiceProtocolType.ICMP, "1.1.1.1", "Cloudflare", "DNS"),
new (ServiceProtocolType.TCP, "1.1.1.1:53", "Cloudflare", "DNS"),
new (ServiceProtocolType.ICMP, "google.pt", "Google", "DNS"),
new (ServiceProtocolType.ICMP, "8.8.8.8", "Google", "DNS"),
new (ServiceProtocolType.TCP, "8.8.8.8:53", "Google", "DNS"),
new (ServiceProtocolType.HTTP, "https://www.google.com", "Google", "WWW"),
new (ServiceProtocolType.HTTP, "https://www.sapo.pt", "Sapo", "WWW"),
new (ServiceProtocolType.ICMP, "9.9.9.9", "Quad9", "DNS"),
new (ServiceProtocolType.ICMP, "76.76.2.0", "Control D", "DNS"),
new (ServiceProtocolType.ICMP, "208.67.222.222", "OpenDNS Home", "DNS"),
new (ServiceProtocolType.ICMP, "94.140.14.14", "AdGuard DNS", "DNS"),
new (ServiceProtocolType.ICMP, "185.228.168.9", "CleanBrowsing", "DNS"),
new (ServiceProtocolType.ICMP, "76.76.19.19", "Alternate DNS", "DNS"),
new (ServiceProtocolType.ICMP, "45.90.28.0", "NextDNS", "DNS"),
new (ServiceProtocolType.ICMP, "8.26.56.26", "Comodo Secure", "DNS"),
new (ServiceProtocolType.ICMP, "192.95.54.3", "OpenNIC", "DNS"),
new (ServiceProtocolType.ICMP, "193.110.81.9", "DNS0", "DNS"),
new (ServiceProtocolType.ICMP, "194.242.2.2", "Mullvad", "DNS"),
new (ServiceProtocolType.ICMP, "microsoft.com", "Microsoft", "DNS")
    ];
#endif
}

public partial class NetworkInterfacesSettings : BaseSettings
{
    public const int RefreshEveryMinSeconds = 1;
    public const int RefreshEveryDefaultSeconds = 5;
    public const int RefreshEveryMaxSeconds = 3600;


    public const int CardMinWidth = 400;
    public const int CardDefaultWidth = 500;
    public const int CardMaxWidth = 1000;

    public const int CardMinHeight = 400;
    public const int CardDefaultHeight = 600;
    public const int CardMaxHeight = 2000;

    [ObservableProperty]
    public partial bool AutoRefresh { get; set; } = true;

    public int RefreshEverySeconds
    {
        get;
        set => SetProperty(ref field, Math.Clamp(value, RefreshEveryMinSeconds, RefreshEveryMaxSeconds));
    } = RefreshEveryDefaultSeconds;

    public int CardWidth
    {
        get;
        set => SetProperty(ref field, Math.Clamp(value, CardMinWidth, CardMaxWidth));
    } = CardDefaultWidth;

    public int CardHeight
    {
        get;
        set => SetProperty(ref field, Math.Clamp(value, CardMinHeight, CardMaxHeight));
    } = CardDefaultHeight;

    [ObservableProperty]
    public partial bool EnableFilterTypes { get; set; } = false;

    [ObservableProperty]
    public partial ObservableDictionary<NetworkInterfaceType, EnumViewFilter> FilterTypes { get; set; } = new();

    [ObservableProperty]
    public partial bool EnableFilterStatus { get; set; } = true;

    [ObservableProperty]
    public partial ObservableDictionary<OperationalStatus, EnumViewFilter> FilterStatus { get; set; } = new();

    [ObservableProperty]
    public partial bool EnableFilterOthers { get; set; } = true;

    [ObservableProperty]
    public partial bool? FilterByVirtual { get; set; } = false;

    [ObservableProperty]
    public partial bool? FilterByHavePhysicalAddress { get; set; }

    [ObservableProperty]
    public partial bool? FilterByHaveIPAddress { get; set; }

    [ObservableProperty]
    public partial bool? FilterByIsTransmittingData { get; set; }


    public NetworkInterfacesSettings()
    {
    }

    public override void OnLoaded(bool fromFile)
    {
        bool isInit = FilterTypes.Count == 0;
        var interfaceTypes = EnumExtensions.GetAllValues<NetworkInterfaceType>(true);
        foreach (var value in interfaceTypes)
        {
            FilterTypes.TryAdd(value, new(value));
            FilterTypes[value].IconKind = NetworkInterfaceBridge.GetNetworkInterfaceTypeIcon(value);
        }

        FilterTypes.TryAdd((NetworkInterfaceType)53, new("Proprietary virtual/internal"));
        FilterTypes[(NetworkInterfaceType)53].IconKind = NetworkInterfaceBridge.GetNetworkInterfaceTypeIcon((NetworkInterfaceType)53);

        if (isInit)
        {
            foreach (var value in FilterTypes)
            {
                value.Value.Include = value.Key
                    is NetworkInterfaceType.Ethernet3Megabit
                    or NetworkInterfaceType.FastEthernetT
                    or NetworkInterfaceType.FastEthernetFx
                    or NetworkInterfaceType.Ethernet
                    or NetworkInterfaceType.GigabitEthernet
                    or NetworkInterfaceType.Wireless80211
                    or NetworkInterfaceType.VeryHighSpeedDsl
                    or NetworkInterfaceType.AsymmetricDsl
                    or NetworkInterfaceType.MultiRateSymmetricDsl
                    or NetworkInterfaceType.RateAdaptDsl
                    or NetworkInterfaceType.SymmetricDsl;
            }
        }


        //isInit = FilterStatus.Count == 0;
        var statusValues = EnumExtensions.GetAllValues<OperationalStatus>(true);
        foreach (var value in statusValues)
        {
            FilterStatus.TryAdd(value, new(value));
            FilterStatus[value].IconKind = NetworkInterfaceBridge.GetStatusIcon(value);
        }

        /*if (isInit)
        {
            foreach (var value in FilterStatus)
            {
                value.Value.Include = value.Key
                    is OperationalStatus.Up
                    or OperationalStatus.Testing;
            }
        }*/
    }
}

public partial class AppSettings : BaseSettings
{
    #region Constants
    public const string Section = "AppSettings";
    #endregion

    #region Singleton
    private static readonly Lazy<AppSettings> LazyInstance = new(() =>
    {
        bool loadFromFile = false;
        AppSettings? settings = null;
        try
        {
            if (File.Exists(App.SettingsFile))
            {

                using var stream = File.OpenRead(App.SettingsFile);
                settings = JsonSerializer.Deserialize<AppSettings>(stream, App.JsonSerializerOptions);
                loadFromFile = settings is not null;
            }
        }
        catch (Exception e)
        {
            App.HandleSafeException(e, $"Read {Path.GetFileName(App.SettingsFile)}");
        }

        settings ??= new AppSettings();
        settings.OnLoaded(loadFromFile);
        settings.PingServices.OnLoaded(loadFromFile);
        settings.NetworkInterfaces.OnLoaded(loadFromFile);
        return settings;
    });

    public static AppSettings Instance => LazyInstance.Value;
    #endregion

    [ObservableProperty]
    public partial ApplicationTheme Theme { get; set; } = ApplicationTheme.Default;

    [ObservableProperty]
    public partial string ThemeColor { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool BackgroundAnimations { get; set; } = true;

    [ObservableProperty]
    public partial bool BackgroundTransitions { get; set; } = true;

    [ObservableProperty]
    public partial SukiBackgroundStyle BackgroundStyle { get; set; } = SukiBackgroundStyle.GradientSoft;

    [ObservableProperty]
    public partial bool CheckForUpdates { get; set; } = true;

    [ObservableProperty]
    public partial DateTime LastUpdateDateTimeCheck { get; set; } = App.Born;

    [ObservableProperty]
    public partial bool IsSideMenuExpanded { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of concurrent tasks enabled by this <see cref="T:System.Threading.Tasks.ParallelOptions" /> instance.
    /// </summary>
    public int MaxDegreeOfParallelism
    {
        get;
        set => SetProperty(ref field, Math.Max(-1, value));
    } = -1;

    /// <summary>
    /// Gets or sets the last window state.
    /// </summary>
    [ObservableProperty]
    public partial WindowState LastWindowState { get; set; } = WindowState.Maximized;

    [ObservableProperty]
    public partial PingServicesSettings PingServices { get; set; } = new();

    [ObservableProperty]
    public partial NetworkInterfacesSettings NetworkInterfaces { get; set; } = new();


    public void Save()
    {
        try
        {
            Directory.CreateDirectory(App.ConfigPath);
            /*if (File.Exists(App.SettingsFile))
            {
                // Backup
                File.Copy(App.SettingsFile, Path.Combine(App.ConfigPath, $"{Path.GetFileName(App.SettingsFile)}_{DateTime.Now:yyyyMMdd-HHmmss}.bak"), true);
            }*/
            using var stream = File.Create(App.SettingsFile);
            JsonSerializer.Serialize(stream, this, App.JsonSerializerOptions);
        }
        catch (Exception e)
        {
            App.HandleSafeException(e, $"Save {Path.GetFileName(App.SettingsFile)}");
        }
    }
}