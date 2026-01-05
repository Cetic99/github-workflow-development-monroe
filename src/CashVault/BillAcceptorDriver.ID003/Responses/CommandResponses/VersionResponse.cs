using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses.CommandResponses;

public class VersionResponse : BaseResponse, IDeviceCommandResponse
{
    public string VersionInfo { get; init; }
    public VersionResponse() : base(0x88) { }
    
    public VersionResponse(byte[] data) : base(0x88)
    {
        VersionInfo = Encoding.ASCII.GetString(data);
    }
}
