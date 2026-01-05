using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses.WorkingStatusResponses;

public class RejectingResponse : BaseResponse, IWorkingStatusResponse
{
    public string RejectingReason { get; private set; }

    public RejectingResponse() : base(0x17)
    { }

    public RejectingResponse(byte[] data): base(0x17)
    {
        if (data.Length != 1)
        {
            throw new ArgumentException("Data length must be 1");
        }

        RejectingReason = data[0] switch
        {
            0x71 => "Insertion error",
            0x72 => "MagneticPattern error (Center)",
            0x73 => "Return action due to residual bills, etc. (at the head part of acceptor)",
            0x74 => "Calibration error / Magnetic Pattern error",
            0x75 => "Conveying error",
            0x76 => "Discrimination error for bill denomination",
            0x77 => "Photo pattern error (1)",
            0x78 => "Photo level error",
            0x79 => "Return by INHIBIT: Error of insertion direction / Error of bill denomination / No command sent answering to ESCROW",
            0x7B => "Operation error",
            0x7C => "Return action due to resudual bills, etc. (at the stacker)",
            0x7D => "Length error",
            0x7E => "Photo pattern error (2)",
            0x7F => "True bill feature error",
            _ => "Unknown"
        };
        

    }

    public override string ToString()
    {
        return $"Reason: {RejectingReason}";
    }
}