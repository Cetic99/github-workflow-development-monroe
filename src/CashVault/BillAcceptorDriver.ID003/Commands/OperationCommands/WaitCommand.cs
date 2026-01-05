using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Commands.OperationCommands;

internal class WaitCommand : BaseCommand, IOperationCommand
{
    public WaitCommand() : base(0x45) { }
}
