using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Commands.OperationCommands;

public class Stack1Command : BaseCommand, IOperationCommand
{
    public Stack1Command() : base(0x41) { }
}
