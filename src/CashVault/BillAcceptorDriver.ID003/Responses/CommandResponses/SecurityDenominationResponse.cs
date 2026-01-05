using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses.CommandResponses;

public class SecurityDenominationResponse : BaseResponse, IDeviceCommandResponse
{
    public SecurityDenominationResponse() : base(0xC1) { }

    public SecurityDenominationResponse(byte[] data) : base(0xC1)
    {
        HasData = true;
        Data = data;
    }
}