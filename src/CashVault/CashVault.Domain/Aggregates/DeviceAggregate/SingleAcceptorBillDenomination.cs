using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.DeviceAggregate;

public class SingleAcceptorBillDenomination
{
    /// <summary>
    /// Currency of the bill
    /// </summary>
    public Currency Currency { get; set; }

    /// <summary>
    /// Data byte from ESCROW response. Should be between: 0x61 and 0x69, 0x71 and 0x79.
    /// </summary>
    public byte DataKey { get; set; }

    /// <summary>
    /// Actual value of the bill
    /// </summary>
    public decimal DataValue { get; set; }

    /// <summary>
    /// Is the bill denomination enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

    public SingleAcceptorBillDenomination(Currency currency, byte dataKey, decimal dataValue, bool isEnabled)
    {
        Currency = currency;
        DataKey = dataKey;
        DataValue = dataValue;
        IsEnabled = isEnabled;
    }

    public SingleAcceptorBillDenomination(Currency currency, byte dataKey, decimal dataValue)
    {
        Currency = currency;
        DataKey = dataKey;
        DataValue = dataValue;
        IsEnabled = false;
    }

    public SingleAcceptorBillDenomination()
    {
    }

}