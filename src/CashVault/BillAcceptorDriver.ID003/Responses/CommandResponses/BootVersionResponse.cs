using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses.CommandResponses;

public class BootVersionResponse : BaseResponse, IDeviceCommandResponse
{
    public BootVersionResponse() : base(0x89) { }

    public BootVersionResponse(byte[] data) : base(0x89)
    {
        // TODO: Do something with the data
    }
}
