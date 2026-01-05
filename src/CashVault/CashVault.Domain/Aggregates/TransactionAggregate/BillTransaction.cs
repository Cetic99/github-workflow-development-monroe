using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.TransactionAggregate;

public abstract class BillTransaction : Transaction
{
    protected BillTransaction() : base()
    {
    }

    protected BillTransaction(
        decimal amountRequested,
        TransactionType type,
        string description,
        decimal previousCreditAmount,
        string? externalReference = null,
        Currency? currency = null)
        : base(amountRequested, type, description, previousCreditAmount, externalReference, currency)
    {
    }
}
