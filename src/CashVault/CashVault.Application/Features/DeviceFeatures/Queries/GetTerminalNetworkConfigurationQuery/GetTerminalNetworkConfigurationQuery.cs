using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries;

public record GetTerminalNetworkConfigurationQuery : IRequest<List<NetworkAdapterDto>> { }

internal sealed class GetTerminalNetworkConfigurationQueryHandler : IRequestHandler<GetTerminalNetworkConfigurationQuery, List<NetworkAdapterDto>>
{
    private readonly ITerminal _terminal;

    public GetTerminalNetworkConfigurationQueryHandler(ITerminal terminal)
    {
        _terminal = terminal;
    }

    public Task<List<NetworkAdapterDto>> Handle(GetTerminalNetworkConfigurationQuery request, CancellationToken cancellationToken)
    {
        List<NetworkAdapterDto> adapters = [];
        NetworkConfiguration networkConfiguration = _terminal.NetworkConfiguration;

        foreach (NetworkInterface ni in NetworkConfiguration.GetSupportedNetworkInterfaces())
        {
            NetworkAdapterConfiguration? matchedNetworkConfiguration =
                networkConfiguration.NetworkAdaptersConfig.FirstOrDefault(x => x.Name == ni.Name);

            var properties = ni.GetIPProperties();

            bool isDhcpEnabled = GetIsDhcpEnabled(ni, matchedNetworkConfiguration);
            bool isDnsAuto = matchedNetworkConfiguration?.IsDnsEnabled ?? true;

            var dnsServers = properties.DnsAddresses.Select(dns => dns.ToString()).ToList();

            string? preferredDns = isDnsAuto ? null : (matchedNetworkConfiguration?.PreferredDns ?? dnsServers.FirstOrDefault());
            string? alternateDns = isDnsAuto ? null : (matchedNetworkConfiguration?.AlternateDns ?? (dnsServers.Count > 1 ? dnsServers[1] : null));

            bool hasAdminPrivilages = Terminal.AppHasAdminPrivilages();

            var adapter = new NetworkAdapterDto
            {
                Name = ni.Name,
                Description = ni.Description,
                Status = ni.OperationalStatus.ToString(),
                HasAdminPrivilages = hasAdminPrivilages,
                IsDnsEnabled = isDnsAuto,
                IsDhcpEnabled = isDhcpEnabled,
                MacAddress = BitConverter.ToString(ni.GetPhysicalAddress().GetAddressBytes()).ToString(),
                NetworkAdapterInfo = new NetworkAdapterInfoDto
                {
                    IpAddress = properties.UnicastAddresses
                        .FirstOrDefault(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork)?.Address.ToString(),
                    NetworkMask = properties.UnicastAddresses
                        .FirstOrDefault(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork)?.IPv4Mask.ToString(),
                    Gateway = properties.GatewayAddresses.FirstOrDefault()?.Address.ToString(),
                    PreferredDns = preferredDns,
                    AlternateDns = alternateDns
                }
            };

            adapters.Add(adapter);
        }
        return Task.FromResult(adapters);
    }

    /// <summary>
    /// Gets the DHCP enabled status for a network interface.
    /// On Linux, the .NET API does not support IsDhcpEnabled, so we use nmcli to check.
    /// </summary>
    private static bool GetIsDhcpEnabled(NetworkInterface ni, NetworkAdapterConfiguration? savedConfig)
    {
        // If we have a saved configuration, use it
        if (savedConfig != null)
        {
            return savedConfig.IsDhcpEnabled;
        }

        // On Windows, use the native API
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                var ipv4Properties = ni.GetIPProperties().GetIPv4Properties();
                return ipv4Properties.IsDhcpEnabled;
            }
            catch (PlatformNotSupportedException)
            {
                return true; // Default to DHCP enabled
            }
        }

        // On Linux, use nmcli to detect DHCP status
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return GetLinuxDhcpStatusViaNmcli(ni.Name);
        }

        return true; // Default to DHCP enabled
    }

    /// <summary>
    /// Determines DHCP status on Linux using nmcli (NetworkManager CLI).
    /// </summary>
    private static bool GetLinuxDhcpStatusViaNmcli(string interfaceName)
    {
        try
        {
            // First, get the connection name for this interface
            string? connectionName = GetNmcliConnectionName(interfaceName);
            if (string.IsNullOrEmpty(connectionName))
            {
                return true; // Default to DHCP if no connection found
            }

            // Get the IPv4 method for this connection
            var processInfo = new ProcessStartInfo
            {
                FileName = "nmcli",
                Arguments = $"-g ipv4.method connection show \"{connectionName}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            if (process == null)
            {
                return true; // Default to DHCP if process failed to start
            }

            string output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                return true; // Default to DHCP if command failed
            }

            // "auto" means DHCP, "manual" means static IP
            return output.Equals("auto", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return true; // Default to DHCP enabled on any error
        }
    }

    /// <summary>
    /// Gets the NetworkManager connection name for a given interface.
    /// </summary>
    private static string? GetNmcliConnectionName(string interfaceName)
    {
        try
        {
            // Get active connection name for the interface
            var processInfo = new ProcessStartInfo
            {
                FileName = "nmcli",
                Arguments = $"-t -f NAME,DEVICE connection show --active",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            if (process == null)
            {
                return null;
            }

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                return null;
            }

            // Parse output - format is "ConnectionName:DeviceName" per line
            foreach (var line in output.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = line.Split(':');
                if (parts.Length >= 2 && parts[1].Trim().Equals(interfaceName, StringComparison.OrdinalIgnoreCase))
                {
                    return parts[0].Trim();
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}
