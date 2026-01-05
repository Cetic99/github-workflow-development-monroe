using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses.CommandResponses;

internal class CommunicationModeResponse : BaseResponse, IDeviceCommandResponse
{
    public CommunicationModeResponse() : base(0xC2) { }

    public CommunicationModeResponse(byte[] data) : base(0xC2)
    {
        HasData = true;
        Data = data;
    }
}
