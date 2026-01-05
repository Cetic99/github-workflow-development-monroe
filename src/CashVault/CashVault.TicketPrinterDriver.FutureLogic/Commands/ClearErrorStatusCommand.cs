using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.TicketPrinterDriver.FutureLogic.Commands
{
    internal class ClearErrorStatusCommand : BaseCommand
    {
        public ClearErrorStatusCommand() : base()
        {

        }
        public override byte[] GetMessageBytes()
        {
            return System.Text.Encoding.ASCII.GetBytes("^C|j|^");
        }
    }
}
