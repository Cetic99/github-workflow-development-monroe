namespace CashVault.Application.Features.ParcelLockerFeatures.Queries;

public class PostalServiceLocationDto
{
    public Guid Uuid { get; set; }
    public string PostalService { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string City { get; set; } = string.Empty;
    public string StreetName { get; set; } = string.Empty;
    public string StreetNumber { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
