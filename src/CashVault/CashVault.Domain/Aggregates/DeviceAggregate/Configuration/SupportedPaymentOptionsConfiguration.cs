using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using CashVault.Domain.Common;
using System.Collections.Generic;
using System.Linq;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Configuration;

public class SupportedPaymentOptionsConfiguration
{
    public List<PaymentOption> PaymentOptions { get; set; } = [];

    public SupportedPaymentOptionsConfiguration() { }

    public void Initialize()
    {
        if (PaymentOptions is null || PaymentOptions.Count == 0)
        {
            PaymentOptions = Enumeration.GetAll<PaymentOption>().ToList();
        }
    }
}