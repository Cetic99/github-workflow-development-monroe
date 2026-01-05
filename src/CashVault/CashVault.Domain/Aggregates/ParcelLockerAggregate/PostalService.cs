using CashVault.Domain.Common;
using CashVault.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace CashVault.Domain.Aggregates.ParcelLockerAggregate;

public class PostalServiceDeliveryOption
{
    public DeliveryOption DeliveryOption { get; set; }
    public bool IsEnabled { get; set; }
    public bool RequiresPayment { get; set; }
    public decimal Amount { get; set; }
    public Currency? Currency { get; set; }
    public ParcelLockerSize ParcelLockerSize { get; set; }

    public PostalServiceDeliveryOption() { }

    public PostalServiceDeliveryOption(
        DeliveryOption deliveryOption,
        bool enabled,
        bool requiresPayment,
        decimal amount,
        Currency? currency,
        ParcelLockerSize parcelLockerSize)
    {
        DeliveryOption = deliveryOption;
        IsEnabled = enabled;
        RequiresPayment = requiresPayment;
        Amount = amount;
        Currency = currency;
        ParcelLockerSize = parcelLockerSize;
    }
}

public class PostalService : Entity
{
    public string Name { get; set; }
    public string Code { get; set; }
    public int DisplaySequence { get; set; } = 9999;
    public List<PostalServiceDeliveryOption> DeliveryOptions { get; set; } = [];

    public PostalService() { }

    public PostalService(string code, string name)
    {
        Name = name;
        Code = code;
    }

    public void UpdateDetails(string name, string code, int displaySequence)
    {
        Name = name;
        Code = code;
        DisplaySequence = displaySequence;
    }

    public void SetDeliveryOptions(List<PostalServiceDeliveryOption> deliveryOptions)
    {
        DeliveryOptions = deliveryOptions;
    }

    public PostalServiceDeliveryOption? GetDeliveryOption(
        DeliveryOption? deliveryOption,
        ParcelLockerSize? lockerSize)
    {
        if (deliveryOption is null || lockerSize is null ||
             DeliveryOptions is null || DeliveryOptions.Count == 0)
            return null;

        return DeliveryOptions.FirstOrDefault(x => x.DeliveryOption.Equals(deliveryOption) &&
            x.ParcelLockerSize.Equals(lockerSize));
    }
}
