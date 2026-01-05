using CashVault.Domain.Common;

namespace CashVault.Domain.Aggregates.ParcelLockerAggregate;

// TODO: Review this
public class PostalServiceLocationType : Enumeration
{
    public static readonly PostalServiceLocationType ParcelLocker = new(1, nameof(ParcelLocker));
    public static readonly PostalServiceLocationType PostalOffice = new(2, nameof(PostalOffice));
    public static readonly PostalServiceLocationType ParcelShop = new(3, nameof(ParcelShop));
    // something else as type?

    public PostalServiceLocationType(int id, string code) : base(id, code) { }
}
