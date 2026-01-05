using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Commands.DeviceCommands;

public class SetBarcodeFunctionsCommand : BaseCommand, IDeviceCommand
{
    public SetBarcodeFunctionsCommand(Span<byte> data) : base(0xC6, data) { }
}
