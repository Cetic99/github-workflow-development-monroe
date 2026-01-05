using System.Collections.Generic;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

public interface ITITOPrinterConfiguration : IBasicHardwareDeviceConfiguration
{
    public bool IsCasinoManagementSystem { get; }
    public int BaudRate { get; set; }
    public bool HasTemplate0 { get; set; }
    public List<int> SupportedBaudRates { get; set; }
    public bool WaitForTicketTaking { get; set; }
    public int TicketTakingTimeout { get; set; }
}
