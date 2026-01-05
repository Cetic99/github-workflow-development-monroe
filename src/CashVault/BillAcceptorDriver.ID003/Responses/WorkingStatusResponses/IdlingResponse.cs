using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses.WorkingStatusResponses;

public class IdlingResponse : BaseResponse, IWorkingStatusResponse
{
    public IdlingResponse() : base(0x11)
    { }
}
