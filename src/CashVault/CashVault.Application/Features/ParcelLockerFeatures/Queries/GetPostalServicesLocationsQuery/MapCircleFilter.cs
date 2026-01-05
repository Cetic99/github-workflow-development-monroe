namespace CashVault.Application.Features.ParcelLockerFeatures.Queries;

public class MapCircleFilter
{
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public decimal Radius { get; set; } // in km
    // precision ?
}
