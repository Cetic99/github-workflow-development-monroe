using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

namespace CashVault.TicketPrinterDriver.FutureLogic.Config;

public class TITOPrinterFutureLogicConfiguration : ITITOPrinterConfiguration
{
    public bool IsEnabled { get; set; } = false;

    public bool IsCasinoManagementSystem { get; set; } = false;
    public int BaudRate { get; set; }
    public bool HasTemplate0 { get; set; }
    public List<int> SupportedBaudRates { get; set; } = [1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200];
    public bool WaitForTicketTaking { get; set; }
    public int TicketTakingTimeout { get; set; }

    public TITOPrinterFutureLogicConfiguration()
    {
        BaudRate = 38400;
        HasTemplate0 = true;
        WaitForTicketTaking = false;
        TicketTakingTimeout = 200_000;
    }

    public TITOPrinterFutureLogicConfiguration(int baudRate, bool hasTemplate0, bool isCasinoManagementSystem, bool waitForTicketTaking, int ticketTakingTimeout)
    {
        BaudRate = baudRate;
        HasTemplate0 = hasTemplate0;
        IsCasinoManagementSystem = isCasinoManagementSystem;
        WaitForTicketTaking = waitForTicketTaking;
        TicketTakingTimeout = ticketTakingTimeout;
    }

    public virtual void Validate()
    {
        if (!SupportedBaudRates.Contains(BaudRate))
        {
            throw new ArgumentException(
                $"BaudRate must be one of the following values: {string.Join(", ", SupportedBaudRates)}."
            );
        }

        if (TicketTakingTimeout < 10_000)
        {
            throw new ArgumentException("TicketTakingTimeout must be greater than or equal to 10,000 milliseconds.");
        }
    }
}