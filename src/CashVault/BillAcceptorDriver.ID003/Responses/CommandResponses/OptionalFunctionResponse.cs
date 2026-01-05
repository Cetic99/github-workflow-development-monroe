using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CashVault.BillAcceptorDriver.ID003.Responses.CommandResponses;

public class OptionalFunctionResponse : BaseResponse, IDeviceCommandResponse
{
    public OptionalFunctionResponse() : base(0xC5) { }

    public OptionalFunctionResponse(byte[] data) : base(0xC5)
    {
        HasData = true;
        Data = data;
    }
}
