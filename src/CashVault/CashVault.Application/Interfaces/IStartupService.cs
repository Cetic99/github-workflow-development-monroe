using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

namespace CashVault.Application.Interfaces;

public interface IStartupService
{
    void OnApplicationStarted();
    void OnApplicationStopping();
    void AddEventDispatching(ITerminal terminal);
}
