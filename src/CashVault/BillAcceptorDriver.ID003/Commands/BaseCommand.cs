using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Commands;

public abstract class BaseCommand : IAcceptorCommand
{
    public byte Key { get; private set; }
    public bool HasData { get; protected set; } = false;
    public Memory<byte> Data { get; protected set; }

    public DateTime TimeStamp => throw new NotImplementedException();

    public BaseCommand(byte key)
    {
        Key = key;
    }

    public BaseCommand(byte key, Span<byte> data)
    {
        Key = key;
        HasData = true;
        Data = data.ToArray();
    }

    public byte[] GetMessageBytes()
    {
        if (HasData)
        {
            var res = new byte[Data.Length + 1];
            
            res[0] = Key;
            Array.Copy(Data.ToArray(), 0, res, 1, Data.Length);
            
            return res;
        }
        else
        {
            return [Key];
        }
    }
}
