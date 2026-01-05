using System;
using System.Collections.Generic;
using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.DeviceAggregate;

public class BillAcceptorCurrencyBillDenomination
{
    public Currency currency { get; init; }
    public List<SingleAcceptorBillDenomination> BillDenominations { get; init; }

    public BillAcceptorCurrencyBillDenomination(Currency? currency, List<SingleAcceptorBillDenomination>? billDenominations)
    {
        this.currency = currency ?? throw new ArgumentNullException(nameof(currency));
        BillDenominations = billDenominations ?? throw new ArgumentNullException(nameof(billDenominations));
    }
}