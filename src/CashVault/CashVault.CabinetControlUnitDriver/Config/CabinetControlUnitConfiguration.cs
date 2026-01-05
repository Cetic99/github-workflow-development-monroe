using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

namespace CashVault.CabinetControlUnitDriver.Config;

public class CabinetControlUnitConfiguration : ICabinetConfiguration
{
    public bool IsEnabled { get; set; } = false;

    public int BaudRate { get; set; }

    public CabinetControlUnitConfiguration()
    {
        BaudRate = 19200;
    }

    public CabinetControlUnitConfiguration(int baudRate)
    {
        BaudRate = baudRate;
    }

    public virtual void Validate()
    {
        if (BaudRate != 1200 && BaudRate != 2400 && BaudRate != 4800 && BaudRate != 9600 && BaudRate != 19200 && BaudRate != 38400 && BaudRate != 57600 && BaudRate != 115200)
        {
            throw new ArgumentException("BaudRate must be one of the following values: 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200.");
        }
    }
}
