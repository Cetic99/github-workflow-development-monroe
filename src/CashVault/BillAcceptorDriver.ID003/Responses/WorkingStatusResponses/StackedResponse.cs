using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses.WorkingStatusResponses;

public class StackedResponse : BaseResponse, IWorkingStatusResponse
{
    public StackedResponse() : base(0x16)
    { }
}