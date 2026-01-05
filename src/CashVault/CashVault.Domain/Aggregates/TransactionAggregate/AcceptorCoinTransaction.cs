using CashVault.Domain.TransactionAggregate;
using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.TransactionAggregate;

public class AcceptorCoinTransaction : CoinTransaction
{
    public AcceptorCoinTransaction(
        decimal amount,
        decimal previousCreditAmount,
        string description = "Coin accepted",
        string? externalReference = null,
        Currency? currency = null)
        : base(
            amount,
            TransactionType.Credit,
            description,
            previousCreditAmount,
            externalReference,
            currency)
    { }
}