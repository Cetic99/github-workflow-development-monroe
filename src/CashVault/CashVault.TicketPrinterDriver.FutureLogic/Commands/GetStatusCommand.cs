using CashVault.DeviceDriver.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.TicketPrinterDriver.FutureLogic.Commands;

internal class GetStatusCommand : BaseCommand
{
    public GetStatusCommand() : base()
    {
    }

    public override byte[] GetMessageBytes()
    {
        return System.Text.Encoding.ASCII.GetBytes("^Se|^");
    }
}
