using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.TicketPrinterDriver.FutureLogic.Commands
{
    internal class ResetPrinterCommand : BaseCommand
    {
        public ResetPrinterCommand() : base()
        {
        }

        public override byte[] GetMessageBytes()
        {
            return System.Text.Encoding.ASCII.GetBytes("^r|^");
        }
    }
}
