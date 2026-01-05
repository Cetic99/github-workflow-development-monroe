using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses;

internal class InvalidCommandResponse : BaseResponse
{
    public InvalidCommandResponse() : base(0x4B)
    { }
}
