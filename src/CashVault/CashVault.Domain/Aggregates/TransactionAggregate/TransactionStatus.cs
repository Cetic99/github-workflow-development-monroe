using CashVault.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.Domain.TransactionAggregate;

public class TransactionStatus : Enumeration
{
    public static readonly TransactionStatus Pending = new(1, nameof(Pending).ToLowerInvariant());
    public static readonly TransactionStatus Completed = new(2, nameof(Completed).ToLowerInvariant());
    public static readonly TransactionStatus PartiallyCompleted = new(3, nameof(PartiallyCompleted).ToLowerInvariant());
    public static readonly TransactionStatus Failed = new(4, nameof(Failed).ToLowerInvariant());

    public TransactionStatus(int id, string code) : base(id, code)
    {
    }
}
