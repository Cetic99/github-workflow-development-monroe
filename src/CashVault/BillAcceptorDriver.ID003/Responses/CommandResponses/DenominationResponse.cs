using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses.CommandResponses;

internal class DenominationResponse : BaseResponse, IDeviceCommandResponse
{
    public DenominationResponse() : base(0x8A) { }

    public DenominationResponse(byte[] data) : base(0x8A)
    {
        // TODO: Do something with the data
    }
}
