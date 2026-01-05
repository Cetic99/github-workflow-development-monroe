using CashVault.Domain.TransactionAggregate;
using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.TransactionAggregate;

public class AcceptorBillTransaction : BillTransaction
{
    public AcceptorBillTransaction(
        decimal amount,
        decimal previousCreditAmount,
        string description = "Bill accepted",
        string? externalReference = null,
        Currency? currency = null)
        : base(amount, TransactionType.Credit, description, previousCreditAmount, externalReference, currency)
    {
    }
}
