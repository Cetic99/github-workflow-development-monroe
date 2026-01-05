using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Commands.OperationCommands;

public class ReturnCommand : BaseCommand, IOperationCommand
{
    public ReturnCommand() : base(0x43) { }
}
