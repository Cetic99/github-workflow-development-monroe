using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses.CommandResponses;

public class InhibitResponse : BaseResponse, IDeviceCommandResponse
{
    public InhibitResponse() : base(0xC3) { }

    public InhibitResponse(byte[] data) : base(0xC3)
    {
        HasData = true;
        Data = data;
    }
}
