namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class TerminalUPSConfigurationDto
    {
        public string? ConfiguredUpsType { get; set; }
        public int? ShutdownDelay { get; set; }
        public int? StartupDelay { get; set; }
        public int? UptimeAfterPowerShortage { get; set; }
    }
}