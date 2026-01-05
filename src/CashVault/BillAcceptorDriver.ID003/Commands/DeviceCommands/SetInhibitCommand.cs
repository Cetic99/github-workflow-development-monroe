using CashVault.BillAcceptorDriver.ID003.Interfaces;
using CashVault.DeviceDriver.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CashVault.BillAcceptorDriver.ID003.Commands.DeviceCommands;

public class SetInhibitCommand : BaseCommand, IDeviceCommand
{
    // TODO: refactor this to receive more human readable data than a byte array
    public SetInhibitCommand(bool acceptorEnabled = true) : base(0xC3)
    {
        byte configData = 0x00;
        
        if (acceptorEnabled == false)
        {
            configData = ByteHelper.SetBit(configData, 0);
        }

        this.HasData = true;
        Data = new Memory<byte>([configData]);
    }
}
