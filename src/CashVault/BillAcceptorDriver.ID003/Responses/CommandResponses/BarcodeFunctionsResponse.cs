using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses.CommandResponses;

public class BarcodeFunctionsResponse : BaseResponse, IDeviceCommandResponse
{
    public BarcodeFunctionsResponse() : base(0xC6) { }

    public BarcodeFunctionsResponse(byte[] data) : base(0xC6)
    {
        HasData = true;
        Data = data;
    }
}
