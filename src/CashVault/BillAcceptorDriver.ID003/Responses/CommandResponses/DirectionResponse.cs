using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses.CommandResponses;

internal class DirectionResponse : BaseResponse, IDeviceCommandResponse
{
    public DirectionResponse() : base(0xC4) { }

    public DirectionResponse(byte[] data) : base(0xC4)
    {
        HasData = true;
        Data = data;
    }
}
