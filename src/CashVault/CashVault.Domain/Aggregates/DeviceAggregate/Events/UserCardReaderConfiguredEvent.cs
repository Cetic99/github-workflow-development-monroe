using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events;

public class UserCardReaderConfiguredEvent : DeviceEvent
{
    public IUserCardReaderConfiguration UserCardReaderConfiguration { get; private set; }

    public UserCardReaderConfiguredEvent(IUserCardReaderConfiguration userCardReaderConfiguration)
        : base("Card reader configured", DeviceAggregate.DeviceType.UserCardReader)
    {
        UserCardReaderConfiguration = userCardReaderConfiguration;
    }
}
