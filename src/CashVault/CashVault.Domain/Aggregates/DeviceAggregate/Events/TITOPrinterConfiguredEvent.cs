using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events
{
    public class TITOPrinterConfiguredEvent : DeviceEvent
    {
        public TITOPrinterConfiguredEvent(ITITOPrinterConfiguration titoPrinterConfiguration)
            : base("Ticket printer configured", DeviceAggregate.DeviceType.TITOPrinter)
        {
            TITOPrinterConfiguration = titoPrinterConfiguration;
        }
        public ITITOPrinterConfiguration TITOPrinterConfiguration { get; private set; }
    }
}
