using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses.WorkingStatusResponses;

public class DeviceDisabledResponse : BaseResponse, IWorkingStatusResponse
{
    public DeviceDisabledResponse() : base(0x1A)
    { }
}
