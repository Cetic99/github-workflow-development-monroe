using CashVault.Domain.Common;

namespace CashVault.Domain.Aggregates.ParcelLockerAggregate;

public class DeliveryOption : Enumeration
{
    public static readonly DeliveryOption ParcelLocker = new(1, nameof(ParcelLocker));
    public static readonly DeliveryOption Address = new(2, nameof(Address));
    public static readonly DeliveryOption PostOffice = new(3, nameof(PostOffice));

    public DeliveryOption(int id, string code) : base(id, code) { }
}
