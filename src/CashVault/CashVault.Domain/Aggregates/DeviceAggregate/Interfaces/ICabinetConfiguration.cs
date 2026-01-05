namespace CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

public interface ICabinetConfiguration : IBasicHardwareDeviceConfiguration
{
    public int BaudRate { get; set; }
}
