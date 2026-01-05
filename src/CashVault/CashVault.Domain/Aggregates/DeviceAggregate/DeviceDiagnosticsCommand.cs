namespace CashVault.Domain.Aggregates.DeviceAggregate;

public class DeviceDiagnosticsCommand
{
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;

    public DeviceDiagnosticsCommand(string code)
    {
        Code = code;
    }

    public DeviceDiagnosticsCommand(string code, string displayName)
    {
        Code = code;
        DisplayName = displayName;
    }
}
