using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Commands.DeviceCommands;

public class SetOptionalFunctionCommand : BaseCommand, IDeviceCommand
{
    // TODO: refactor this to receive more human readable data than a byte array
    public SetOptionalFunctionCommand(Span<byte> data) : base(0xC5, data) { }
}
