using CashVault.Domain.TransactionAggregate;
using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.TransactionAggregate;

public abstract class CoinTransaction : Transaction
{
    protected CoinTransaction()
        : base()
    {
    }

    protected CoinTransaction(
        decimal amountRequested,
        TransactionType type,
        string description,
        decimal previousCreditAmount,
        string? externalReference = null,
        Currency? currency = null)
        : base(
            amountRequested,
            type,
            description,
            previousCreditAmount,
            externalReference,
            currency)
    { }
}
