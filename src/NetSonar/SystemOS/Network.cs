using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace NetSonar.Avalonia.SystemOS;

public static class Network
{
    /// <summary>
    /// Checks if the device has internet connection
    /// </summary>
    /// <returns></returns>
    public static async Task<bool> HaveInternet(string host = "1.1.1.1", int timeout = 1000)
    {
        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(host, timeout);
            return reply.Status == IPStatus.Success;
        }
        catch (Exception e)
        {
            App.WriteLine(e);
            return false;
        }
    }
}