namespace CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

public interface IBasicHardwareDeviceConfiguration
{
    bool IsEnabled { get; }
    void Validate();
}
