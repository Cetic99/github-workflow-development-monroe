using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.TicketPrinterDriver.FutureLogic.Common
{
    public class PrinterError : PrinterStatusInfo
    {
        public bool IsCritical { get; init; }
        public bool ResetNeeded { get; init; }
        public bool MaintenanceNeeded { get; init; }
    }
}
