using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Commands.DeviceCommands;

public class SetBarcodeInhibitCommand : BaseCommand, IDeviceCommand
{
    public SetBarcodeInhibitCommand(Span<byte> data) : base(0xC7, data) { }
}
