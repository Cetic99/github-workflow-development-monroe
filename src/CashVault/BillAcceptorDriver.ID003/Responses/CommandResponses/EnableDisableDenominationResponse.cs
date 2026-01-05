using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses.CommandResponses;

public class EnableDisableDenominationResponse : BaseResponse, IDeviceCommandResponse
{
    public EnableDisableDenominationResponse() : base(0xC0) { }

    public EnableDisableDenominationResponse(byte[] data) : base(0xC0)
    {
        HasData = true;
        Data = data;
    }
}
