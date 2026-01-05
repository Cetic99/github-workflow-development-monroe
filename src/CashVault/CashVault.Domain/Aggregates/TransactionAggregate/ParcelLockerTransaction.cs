using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using CashVault.Domain.TransactionAggregate;
using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.TransactionAggregate;

public class ParcelLockerTransaction : Transaction
{
    public int ParcelLockerId { get; set; }
    public ParcelLockerShipment Shipment { get; set; }

    public ParcelLockerTransaction() { }

    public ParcelLockerTransaction(
        ParcelLockerShipment shipment,
        ParcelLocker parcelLocker,
        decimal amount,
        string description = "Parcel locker transaction created.",
        string? externalReference = null,
        Currency? currency = null)
        : base(
            amount,
            TransactionType.Credit,
            description, // add description
            previousCreditAmount: 0, // ?
            externalReference, // add external reference
            currency)
    {
        Shipment = shipment;
        ParcelLockerId = parcelLocker.Id;
    }
}
