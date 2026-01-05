using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using CashVault.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Configuration;

public class PostalServicesConfiguration
{
    public List<PostalService> PostalServices { get; set; } = [];

    public PostalServicesConfiguration() { }

    public void Initialize()
    {
        if (PostalServices is null || PostalServices.Count == 0)
        {
            PostalServices = [new("XExpress", "X Express"), new("EuroExpress", "Euro Express"), new("A2B", "A2B")];

            List<PostalServiceDeliveryOption> deliveryOptions = [
                new(DeliveryOption.ParcelLocker, true, true, 5, Currency.Default, ParcelLockerSize.Small),
                new(DeliveryOption.Address, true, true, 5, Currency.Default , ParcelLockerSize.Small),
                new(DeliveryOption.PostOffice, true, true, 5, Currency.Default, ParcelLockerSize.Small),

                new(DeliveryOption.ParcelLocker, true, true, 10, Currency.Default, ParcelLockerSize.Medium),
                new(DeliveryOption.Address, true, true, 10, Currency.Default , ParcelLockerSize.Medium),
                new(DeliveryOption.PostOffice, true, true, 10, Currency.Default, ParcelLockerSize.Medium),

                new(DeliveryOption.ParcelLocker, true, true, 15, Currency.Default, ParcelLockerSize.Large),
                new(DeliveryOption.Address, true, true, 15, Currency.Default , ParcelLockerSize.Large),
                new(DeliveryOption.PostOffice, true, true, 15, Currency.Default, ParcelLockerSize.Large),
            ];

            PostalServices.ForEach(x => x.SetDeliveryOptions(deliveryOptions));
        }
    }

    public PostalService? GetPostalService(string code)
    {
        if (string.IsNullOrWhiteSpace(code) ||
             PostalServices is null ||
              PostalServices.Count == 0)
            return null;

        return PostalServices.FirstOrDefault(x => x.Code.Equals(code));
    }
}
