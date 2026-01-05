using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses.WorkingStatusResponses;

public class EscrowResponse : BaseResponse, IWorkingStatusResponse
{
    public byte DenomKey { get; private set; }
    public bool IsCoupon { get; private set; } = false;
    public string Barcode { get; private set; }

    public EscrowResponse() : base(0x13) { }

    public EscrowResponse(byte[] data) : base(0x13)
    {
        if (data.Length == 0)
        {
            throw new ArgumentException("Data length must more than zero.");
        }

        DenomKey = data[0];

        if (DenomKey == 0x6F && data.Length > 1)
        {
            IsCoupon = true;
            Barcode = Encoding.ASCII.GetString(data.Skip(1).ToArray());
        }
    }

    public override string ToString()
    {
        string description = "Denom key: " + BitConverter.ToString(new byte[] { DenomKey }).Replace("-", "");
        if (IsCoupon)
        {
            description += " Barcode: " + Barcode;
        }
        return description;
    }

}