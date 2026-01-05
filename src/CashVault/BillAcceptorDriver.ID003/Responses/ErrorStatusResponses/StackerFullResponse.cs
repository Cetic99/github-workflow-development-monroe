using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses.ErrorStatusResponses;

public class StackerFullResponse: BaseResponse, IErrorStatusResponse
{
    public StackerFullResponse() : base(0x43)
    { }
}
