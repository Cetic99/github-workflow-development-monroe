using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses.CommandResponses;

internal class BarcodeInhibitResponse : BaseResponse, IDeviceCommandResponse
{
    public BarcodeInhibitResponse() : base(0xC7)
    {
    }

    public BarcodeInhibitResponse(byte[] data) : base(0xC7)
    {
        HasData = true;
        Data = data;
    }
}
