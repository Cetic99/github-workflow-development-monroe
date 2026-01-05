using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses.ErrorStatusResponses;

public class CommunicationErrorResponse: BaseResponse, IErrorStatusResponse
{
    public CommunicationErrorResponse() : base(0x4A)
    { }
}