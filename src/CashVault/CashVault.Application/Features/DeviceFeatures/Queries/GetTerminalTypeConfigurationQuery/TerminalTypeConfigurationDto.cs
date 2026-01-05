namespace CashVault.Application.Features.DeviceFeatures.Queries;

public class TerminalTypeConfigurationDto
{
    public List<string> SelectedTerminalTypes { get; set; } = [];
    public List<string> SupportedDevices { get; set; } = [];
    public Dictionary<string, List<string>> TerminalTypeToDevicesMap { get; set; } = [];
}