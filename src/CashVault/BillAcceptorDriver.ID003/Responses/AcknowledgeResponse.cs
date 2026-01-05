using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses;

public class AcknowledgeResponse : BaseResponse
{
    public AcknowledgeResponse() : base(0x50) { }
}
