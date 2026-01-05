using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using CashVault.Domain.Common;

namespace CashVault.Application.Features.ParcelLockerFeatures.Models;

public class ShipmentModel
{
    public string Barcode { get; set; } = string.Empty;
    public long RegistrationNumber { get; set; }
    public long LockerAccessCode { get; set; }
    public decimal? Amount { get; set; }
    public string ParcelLockerSize { get; set; } = string.Empty;
    public string RecieverPhoneNumber { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public string PostalService { get; set; } = string.Empty;

    // TODO: currency, payment options

    public ShipmentModel() { }

    public ShipmentModel(
        string barcode,
        long registrationNumber,
        long lockerAccessCode,
        decimal? amount,
        string parcelLockerSize,
        string recieverPhoneNumber,
        DateTime expirationDateNumber,
        string postalService)
    {
        Barcode = barcode;
        RegistrationNumber = registrationNumber;
        LockerAccessCode = lockerAccessCode;
        Amount = amount;
        ParcelLockerSize = parcelLockerSize;
        RecieverPhoneNumber = recieverPhoneNumber;
        ExpirationDate = expirationDateNumber;
        PostalService = postalService;
    }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Barcode))
            throw new ArgumentException(nameof(Barcode));

        if (RegistrationNumber <= 0)
            throw new ArgumentException(nameof(RegistrationNumber));

        if (LockerAccessCode <= 0)
            throw new ArgumentException(nameof(LockerAccessCode));

        if (!Enumeration.Contains<ParcelLockerSize>(ParcelLockerSize))
            throw new ArgumentException(nameof(ParcelLockerSize));

        if (ExpirationDate <= DateTime.UtcNow)
            throw new ArgumentException(nameof(ExpirationDate));

        if (string.IsNullOrWhiteSpace(PostalService))
            throw new ArgumentException(nameof(PostalService));
    }
}
