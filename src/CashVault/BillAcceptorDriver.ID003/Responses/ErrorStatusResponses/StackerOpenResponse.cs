using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses.ErrorStatusResponses;

public class StackerOpenResponse: BaseResponse, IErrorStatusResponse
{
    public StackerOpenResponse() : base(0x44)
    { }
}
