using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Commands.OperationCommands;

public class AcknowledgeCommand : BaseCommand, IOperationCommand
{
    public AcknowledgeCommand() : base(0x50) { }
}
