using System.Collections.Generic;
using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

public interface ICoinAcceptorConfiguration : IBasicHardwareDeviceConfiguration
{
    /// <summary>
    /// List of coin denominations that the coin acceptor can accept.
    /// </summary>
    public List<SingleAcceptorCoinDenomination>? CoinDenominationConfig { get; set; }

    /// <summary>
    /// Current configured currency
    /// </summary>
    public Currency CurrentCurrency { get; set; }

    /// <summary>
    /// Method used to obtain which currencies are supported by the coin acceptor.
    /// </summary>
    /// <returns></returns>
    public List<CurrencyCoinDenomination> SupportedCurrencies { get; }
}