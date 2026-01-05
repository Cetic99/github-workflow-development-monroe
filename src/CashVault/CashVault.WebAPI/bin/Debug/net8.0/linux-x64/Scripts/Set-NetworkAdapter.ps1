param (
    [string]$AdapterName,      # Network adapter name
    [string]$IsDhcpEnabled,    # Enable or disable DHCP (as a string)
    [string]$IsDnsEnabled,     # Enable or disable automatic DNS (as a string)
    [string]$IpAddress,        # Static IP address (used only if DHCP is disabled)
    [int]$PrefixLength,        # Prefix Length
    [string]$Gateway,          # Default gateway (used only if DHCP is disabled)
    [string]$PreferredDns,     # Preferred DNS
    [string]$AlternateDns      # Alternate DNS
)

# --------------------------
# OS DETEKCIJA
# --------------------------
if ($IsLinux) {
    #
    # LINUX – NMCLI IMPLEMENTACIJA
    #


    # Optional: samo zaloguj ko je user
    $who = (whoami).Trim()
    Write-Host "Running as user: $who on Linux."

    # 2) nmcli check
    if (-not (Get-Command nmcli -ErrorAction SilentlyContinue)) {
        Write-Error "nmcli not found. Install NetworkManager before using this script."
        exit 1
    }

    # 3) resolve or create connection for this device
    $connectionName = $null

    $active = nmcli -t -f NAME,DEVICE connection show --active |
        Where-Object { $_ -match ":$AdapterName$" } |
        ForEach-Object { ($_ -split ':')[0] }

    if ($active) {
        $connectionName = $active
        Write-Host "Using active connection '$connectionName' for device '$AdapterName'."
    }
    else {
        $any = nmcli -t -f NAME,DEVICE connection show |
            Where-Object { $_ -match ":$AdapterName$" } |
            ForEach-Object { ($_ -split ':')[0] }

        if ($any) {
            $connectionName = $any
            Write-Host "Using existing connection '$connectionName' for device '$AdapterName'."
        }
        else {
            Write-Host "No existing connection for '$AdapterName', creating new connection."
            nmcli connection add type ethernet con-name "$AdapterName" ifname "$AdapterName" | Out-Null
            $connectionName = $AdapterName
        }
    }

    if (-not $connectionName) {
        Write-Error "Failed to resolve or create connection for '$AdapterName'."
        exit 1
    }

    # 4) IPv4 config
    if ($IsDhcpEnabled -eq "False") {
        # STATIC
        if (-not $IpAddress -or -not $PrefixLength) {
            Write-Error "Static config for '$AdapterName' requires IpAddress and PrefixLength."
            exit 1
        }

        $ipWithPrefix = "$IpAddress/$PrefixLength"
        nmcli connection modify "$connectionName" ipv4.method manual ipv4.addresses "$ipWithPrefix"

        if ($Gateway) {
            nmcli connection modify "$connectionName" ipv4.gateway "$Gateway"
        } else {
            nmcli connection modify "$connectionName" ipv4.gateway ""
        }

        Write-Host "Set static IPv4: $ipWithPrefix, GW: $Gateway on '$AdapterName'."
    }
    else {
        # DHCP
        nmcli connection modify "$connectionName" ipv4.method auto

        Write-Host "Set DHCP IPv4 on '$AdapterName'."
    }

    # 5) DNS config
    if ($IsDnsEnabled -eq "False") {
        nmcli connection modify "$connectionName" ipv4.ignore-auto-dns yes

        $dnsList = @()
        if ($PreferredDns) { $dnsList += $PreferredDns }
        if ($AlternateDns) { $dnsList += $AlternateDns }

        if ($dnsList.Count -gt 0) {
            $dnsString = $dnsList -join ","
            nmcli connection modify "$connectionName" ipv4.dns "$dnsString"
            Write-Host "Set manual DNS: $dnsString."
        }
        else {
            nmcli connection modify "$connectionName" ipv4.dns ""
            Write-Host "Manual DNS requested but none provided – cleared ipv4.dns."
        }
    }
    else {
        nmcli connection modify "$connectionName" ipv4.ignore-auto-dns no
        nmcli connection modify "$connectionName" ipv4.dns ""
        Write-Host "Set DNS to automatic on '$AdapterName'."
    }

    # 6) apply changes
    Write-Host "Applying connection '$connectionName'..."
    nmcli connection down "$connectionName" 2>$null | Out-Null
    nmcli connection up "$connectionName"
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to bring up connection '$connectionName'."
        exit 1
    }

    Write-Host "Network configuration successfully updated (Linux)."
    exit 0
}

#
# WINDOWS – TVOJ POSTOJEĆI KOD
#

$IPType = "IPv4"

$adapter = Get-NetAdapter | Where-Object { $_.Name -eq $AdapterName }

if (-not $adapter) {
    Write-Host "Adapter '$AdapterName' not found!"
    exit 1
}

if ($IsDhcpEnabled -eq "False") {
    Set-NetIPInterface -InterfaceIndex $adapter.InterfaceIndex -Dhcp Disabled

    If (($adapter | Get-NetIPConfiguration).IPv4Address.IPAddress) {
        $adapter | Remove-NetIPAddress -AddressFamily $IPType -Confirm:$false
    }
    If (($adapter | Get-NetIPConfiguration).Ipv4DefaultGateway) {
        $adapter | Remove-NetRoute -AddressFamily $IPType -Confirm:$false
    }

    # Set the new static IP address, prefix length, and default gateway
    New-NetIPAddress -InterfaceIndex $adapter.InterfaceIndex -IPAddress $IpAddress -PrefixLength   $PrefixLength -DefaultGateway $Gateway -AddressFamily $IPType
    Write-Host "Static IP Address: $IpAddress, PrefixLength: $PrefixLength, Gateway: $Gateway set for $AdapterName."
} 
else {
    # Set adapter to obtain IP address automatically (DHCP)
    Set-NetIPInterface -InterfaceIndex $adapter.InterfaceIndex -Dhcp Enabled

    # Remove any existing manual gateway (static routes)
    $currentGateways = Get-NetRoute -InterfaceIndex $adapter.InterfaceIndex -DestinationPrefix "0.0.0.0/0" -ErrorAction SilentlyContinue
    if ($currentGateways) {
        foreach ($route in $currentGateways) {
            Remove-NetRoute -InterfaceIndex $adapter.InterfaceIndex -DestinationPrefix "0.0.0.0/0" -Confirm:$false -ErrorAction SilentlyContinue
        }
    }

    Write-Output "IP address and gateway set to automatic (DHCP) for $AdapterName."
}

# DNS Configuration
if ($IsDnsEnabled -eq "False") {
    if($PreferredDns -ne $null -and $AlternateDns -ne $null){
        Write-Host "Setting Preferred DNS to: $PreferredDns"
        Write-Host "Setting Alternate DNS to: $AlternateDns"
        Set-DnsClientServerAddress -InterfaceAlias $AdapterName -ServerAddresses $PreferredDns,$AlternateDns
    }
    elseif($PreferredDns -ne $null -and $AlternateDns -eq $null){
        Write-Host "Setting Preferred DNS to: $PreferredDns"
        Set-DnsClientServerAddress -InterfaceAlias $AdapterName -ServerAddresses $PreferredDns
    }
} 
elseif ($IsDnsEnabled -eq "True") {
    Set-DnsClientServerAddress -InterfaceIndex $adapter.InterfaceIndex -ResetServerAddresses
    Write-Output "DNS set to obtain addresses automatically for $AdapterName."
}

Write-Host "Network configuration successfully updated!"
