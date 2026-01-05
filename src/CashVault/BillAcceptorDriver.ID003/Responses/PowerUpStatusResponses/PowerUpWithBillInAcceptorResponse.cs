using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses.PowerUpStatusResponses;

public class PowerUpWithBillInAcceptorResponse : BaseResponse, IPowerUpStatusResponse
{
    public PowerUpWithBillInAcceptorResponse() : base(0x41)
    { }
}
