using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

namespace CashVault.UserCardReaderDriver.ACR1252U.Config;

public class UserCardReaderACR1252UConfiguration : IUserCardReaderConfiguration
{
    public bool IsEnabled { get; set; }

    public UserCardReaderACR1252UConfiguration()
    {
        IsEnabled = false;
    }

    public virtual void Validate()
    {

    }
}
