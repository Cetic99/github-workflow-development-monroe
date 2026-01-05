using CashVault.Application.Features.ParcelLockerFeatures.Queries;
using CashVault.Application.Features.ParcelLockerFeatures.QueryModels;
using CashVault.Domain.Aggregates.ParcelLockerAggregate;

namespace CashVault.Application.Interfaces.Persistence;

public interface IParcelLockerRepository
{
    Task<ParcelLockerShipment?> GetShipment(string? barcode, long accessCode, string? postalService = null);

    Task<List<PostalServiceLocation>> GetPostalServicesLocations(
        List<string>? postalServices = null,
        List<PostalServiceLocationType>? locationTypes = null,
        string? query = null,
        MapCircleFilter? mapCircle = null,
        bool? forSending = null,
        bool? forReceiving = null);

    Task<List<ShipmentDto>> GetShipments(string postalService, List<ShipmentStatus> statuses);

    void SaveShipment(ParcelLockerShipment shipment);
}
