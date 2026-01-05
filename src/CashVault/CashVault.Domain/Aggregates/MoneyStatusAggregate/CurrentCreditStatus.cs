using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CashVault.Domain.Aggregates.TransactionAggregate.Events;
using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate;

public class CurrentCreditStatus : MoneyStatus
{
    private decimal _amount;
    public decimal Amount
    {
        get { return _amount; }
        private set
        {
            if (value < 0)
            {
                throw new ArgumentException("Amount cannot be negative");
            }

            _amount = value;
        }
    }
    public Currency Currency { get; private set; }

    public CurrentCreditStatus()
    {
        // TODO: set to zero
        Amount = 0;
        Currency = Currency.Default;
    }

    [JsonConstructor]
    public CurrentCreditStatus(decimal amount, Currency? currency = null)
    {
        Amount = amount;
        Currency = currency ?? Currency.Default;
    }

    public void IncreaseAmount(decimal amount)
    {
        Amount += amount;
        AddDomainEvent(new CreditStatusIncreasedEvent(amount));
    }

    public void DecreaseAmount(decimal amount)
    {
        if (Amount < amount)
        {
            throw new InvalidOperationException("Insufficient funds");
        }

        Amount -= amount;
        AddDomainEvent(new CreditStatusDecreasedEvent(amount));
    }

    public override void ToJsonString()
    {
        JsonValue = System.Text.Json.JsonSerializer.Serialize(this);
    }

    public override void Initialize()
    {
        if (JsonValue == null) return;

        CurrentCreditStatus? currentCreditStatus = JsonSerializer.Deserialize<CurrentCreditStatus>(JsonValue);

        if (currentCreditStatus != null)
        {
            Amount = currentCreditStatus.Amount;
            Currency = currentCreditStatus.Currency;
        }
    }
}