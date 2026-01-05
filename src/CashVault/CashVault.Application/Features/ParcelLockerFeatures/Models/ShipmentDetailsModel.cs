namespace CashVault.Application.Features.ParcelLockerFeatures.Models;

public class ShipmentDetailsModel
{
    public string Barcode { get; set; } = string.Empty;
    public long RegistrationNumber { get; set; }
    public long LockerAccessCode { get; set; }
}
