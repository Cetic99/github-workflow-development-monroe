using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Commands.DeviceCommands;

public class SetSecurityDenominationCommand : BaseCommand, IDeviceCommand
{
    // TODO: refactor this to receive more human readable data than a byte array
    public SetSecurityDenominationCommand(Span<byte> data) : base(0xC1, data) { }
}
