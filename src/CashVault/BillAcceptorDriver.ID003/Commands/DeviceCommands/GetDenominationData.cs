using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Commands.DeviceCommands;

internal class GetDenominationData : BaseCommand, IDeviceCommand
{
    public GetDenominationData() : base(0x8A) { }
}
