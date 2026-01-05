namespace CashVault.Application.Features.ParcelLockerFeatures.Models;

// TODO: This will likely be changed
public class CreateShipmentModel
{
    // Recipient
    public string RecipientFirstName { get; set; }
    public string RecipientLastName { get; set; }
    public string RecipientPhoneNumber { get; set; }

    // Location
    public string Address { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }

    // Sender
    public string SenderPhoneNumber { get; set; }
    public string? SenderEmail { get; set; }

    public string PostalService { get; set; }
}
