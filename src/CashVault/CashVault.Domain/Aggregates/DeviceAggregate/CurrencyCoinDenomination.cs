using System;
using System.Collections.Generic;
using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.DeviceAggregate;

public class CurrencyCoinDenomination
{
    public Currency Currency { get; set; }
    public List<SingleAcceptorCoinDenomination> CoinDenominations { get; set; }

    public CurrencyCoinDenomination(Currency? currency, List<SingleAcceptorCoinDenomination>? coinDenominations)
    {
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
        CoinDenominations = coinDenominations ?? throw new ArgumentNullException(nameof(coinDenominations));
    }
}
