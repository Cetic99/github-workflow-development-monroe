using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses.ErrorStatusResponses;

public class FailureResponse: BaseResponse, IErrorStatusResponse
{
    public string FailureDescription { get; private set; }

    public FailureResponse() : base(0x49) { }

    public FailureResponse(byte[] data) : base(0x49)
    {
        if (data.Length != 1)
        {
            throw new ArgumentException("Data length must be 1");
        }

        FailureDescription = data[0] switch
        {
            0xA2 => "Stack motor failure",
            0xA5 => "Transport (feed) motor speed failure",
            0xA6 => "Transport (feed) motor failure",
            0xA8 => "Solenoid failure",
            0xA9 => "PB Unit failure",
            0xAB => "Cash box not ready",
            0xAF => "Validator head remove",
            0xB0 => "BOOT ROM failure",
            0xB1 => "External ROM failure",
            0xB2 => "RAM failure",
            0xB3 => "External ROM writing failure",
            _ => "Unknown"
        };   
    }

    public override string ToString()
    {
        return $"Failure: {FailureDescription}";
    }
}
