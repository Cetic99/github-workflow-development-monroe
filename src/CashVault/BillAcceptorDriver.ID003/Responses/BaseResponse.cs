using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses;

public abstract class BaseResponse : IAcceptorResponse
{
    public byte Key { get; private set; }
    public byte[]? Data { get; protected set; } = null;
    public bool HasData { get; protected set; } = false;

    public DateTime TimeStamp => throw new NotImplementedException();

    public BaseResponse(byte key)
    {
        this.Key = key;
    }

    public virtual void ParseData(Span<byte> data)
    {
        if (HasData)
        {
            ParseDataInternal(data);
        }
    }

    protected virtual void ParseDataInternal(Span<byte> data)
    {
        throw new NotImplementedException();
    }

    public byte[] GetMessageBytes()
    {
        throw new NotImplementedException();
    }
}