using System;
using CashVault.Domain.Common.Exceptions;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate.Exceptions;

public class InsufficientDispenserBillsException : BaseException
{
    public InsufficientDispenserBillsException(string message) : base(message)
    {
    }
}
