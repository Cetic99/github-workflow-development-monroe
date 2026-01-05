using CashVault.Domain.Common;

namespace CashVault.Domain.TransactionAggregate;

public class TransactionType : Enumeration
{
    public static readonly TransactionType Credit = new(1, nameof(Credit).ToLowerInvariant());
    public static readonly TransactionType Debit = new(2, nameof(Debit).ToLowerInvariant());

    public TransactionType(int id, string code) : base(id, code)
    {
    }
}
