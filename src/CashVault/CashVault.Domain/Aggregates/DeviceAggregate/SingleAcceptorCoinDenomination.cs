using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.DeviceAggregate;

public class SingleAcceptorCoinDenomination
{
    public Currency Currency { get; set; }
    public byte Code { get; set; }
    public decimal Value { get; set; }
    public bool IsEnabled { get; set; }

    public SingleAcceptorCoinDenomination() { }

    public SingleAcceptorCoinDenomination(Currency currency, byte code, decimal value)
    {
        Currency = currency;
        Code = code;
        Value = value;
        IsEnabled = true;
    }

    public SingleAcceptorCoinDenomination(
        Currency currency,
        byte code,
        decimal value,
        bool isEnabled)
    {
        Currency = currency;
        Code = code;
        Value = value;
        IsEnabled = isEnabled;
    }
}
