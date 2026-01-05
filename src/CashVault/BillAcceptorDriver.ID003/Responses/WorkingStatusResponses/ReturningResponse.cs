using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses.WorkingStatusResponses;

public class ReturningResponse : BaseResponse, IWorkingStatusResponse
{
    public ReturningResponse() : base(0x18)
    { }

    public override string ToString()
    {
        return "Barcode rejected or invalid denomination settings.";
    }
}
