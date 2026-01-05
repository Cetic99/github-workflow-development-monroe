using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Configuration;

public class NetworkConfiguration
{
    public const string EthernetAdapterName = "Ethernet";
    public const string WirelessAdapterName = "Wi-Fi";

    public List<NetworkAdapterConfiguration> NetworkAdaptersConfig { get; set; } = [];

    public void UpdateNetworkAdapterConfiguration(NetworkAdapterConfiguration adapterConfig)
    {
        NetworkAdapterConfiguration? existingAdapterConfig = NetworkAdaptersConfig.Find(x => x.Name == adapterConfig.Name);

        if (!GetSupportedNetworkInterfaces().Any(x => x.Name == adapterConfig.Name))
        {
            throw new ArgumentException("Network adapter not found.");
        }

        if (existingAdapterConfig != null)
        {
            existingAdapterConfig.IsDhcpEnabled = adapterConfig.IsDhcpEnabled;
            existingAdapterConfig.IsDnsEnabled = adapterConfig.IsDnsEnabled;
            existingAdapterConfig.IpAddress = adapterConfig.IpAddress;
            existingAdapterConfig.NetworkMask = adapterConfig.NetworkMask;
            existingAdapterConfig.Gateway = adapterConfig.Gateway;
            existingAdapterConfig.PreferredDns = adapterConfig.PreferredDns;
            existingAdapterConfig.AlternateDns = adapterConfig.AlternateDns;
        }
        else
        {
            NetworkAdaptersConfig.Add(adapterConfig);
        }
    }

    public static bool IsValidIpAddress(string? ipAddress)
    {
        return !string.IsNullOrEmpty(ipAddress) && IPAddress.TryParse(ipAddress, out _);
    }

    public static List<NetworkInterface> GetSupportedNetworkInterfaces()
    {
        List<NetworkInterface> supportedInterfaces = [];

        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (!string.IsNullOrEmpty(ni.Name))
            {
                supportedInterfaces.Add(ni);
            }
        }
        return supportedInterfaces;
    }

    public static bool IsValidMask(string mask)
    {
        string[] octets = mask.Split('.');

        if (octets.Length != 4)
        {
            return false;
        }

        foreach (string octet in octets)
        {
            if (!int.TryParse(octet, out int value))
            {
                return false;
            }
            if (value < 0 || value > 255)
            {
                return false;
            }
        }

        string binaryMask = "";
        foreach (string octet in octets)
        {
            binaryMask += Convert.ToString(int.Parse(octet), 2).PadLeft(8, '0');
        }

        if (binaryMask.Contains("01"))
        {
            return false;
        }

        return true;
    }

    public static int MaskToPrefixLength(string? mask)
    {
        if (string.IsNullOrEmpty(mask)) return 0;

        string[] octets = mask.Split('.');
        string binaryMask = "";
        foreach (string octet in octets)
        {
            binaryMask += Convert.ToString(int.Parse(octet), 2).PadLeft(8, '0');
        }

        int prefixLength = 0;
        foreach (char bit in binaryMask)
        {
            if (bit == '1')
            {
                prefixLength++;
            }
        }

        return prefixLength;
    }
}

public class NetworkAdapterConfiguration
{
    public string Name { get; set; } = null!;
    public bool IsDhcpEnabled { get; set; }
    public bool IsDnsEnabled { get; set; }
    public string? IpAddress { get; set; }
    public string? NetworkMask { get; set; }
    public string? Gateway { get; set; }
    public string? PreferredDns { get; set; }
    public string? AlternateDns { get; set; }

    public NetworkAdapterConfiguration(string name, bool isDhcpEnabled, bool isDnsEnabled, string? ipAddress, string? networkMask, string? gateway, string? preferredDns, string? alternateDns)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        Name = name;
        IsDhcpEnabled = isDhcpEnabled;
        IsDnsEnabled = isDnsEnabled;
        IpAddress = ipAddress;
        NetworkMask = networkMask;
        Gateway = gateway;
        PreferredDns = preferredDns;
        AlternateDns = alternateDns;
    }
}