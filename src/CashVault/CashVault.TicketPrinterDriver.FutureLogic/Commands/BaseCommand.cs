using CashVault.DeviceDriver.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.TicketPrinterDriver.FutureLogic.Commands;

internal abstract class BaseCommand : ISerialPortMessage
{
    public DateTime TimeStamp { get; private set; }

    public BaseCommand()
    {
        TimeStamp = DateTime.UtcNow;
    }

    public abstract byte[] GetMessageBytes();
    
    public override string ToString()
    {
        return Encoding.ASCII.GetString(GetMessageBytes());
    }
}
