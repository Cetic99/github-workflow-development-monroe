using CashVault.Application.Features.ParcelLockerFeatures.Queries;
using CashVault.Application.Features.ParcelLockerFeatures.QueryModels;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using Microsoft.EntityFrameworkCore;

namespace CashVault.Infrastructure.PersistentStorage.Repositories;

public class ParcelLockerRepository : IParcelLockerRepository
{
    private readonly CashVaultContext _dbContext;

    public ParcelLockerRepository(CashVaultContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ParcelLockerShipment?> GetShipment(string? barcode, long accessCode, string? postalService = null)
    {
        var shipment = await _dbContext.ParcelLockerShipments
            .Where(x => (x.Barcode == barcode || x.LockerAccessCode == accessCode) &&
                        (string.IsNullOrEmpty(postalService) || postalService == x.PostalService))
            .FirstOrDefaultAsync();

        return shipment;
    }

    public async Task<List<PostalServiceLocation>> GetPostalServicesLocations(
        List<string>? postalServices = null,
        List<PostalServiceLocationType>? locationTypes = null,
        string? query = null,
        MapCircleFilter? mapCircle = null,
        bool? forSending = null,
        bool? forReceiving = null)
    {
        var locationsQuery = _dbContext.PostalServiceLocations.AsQueryable();

        if (forSending.HasValue)
            locationsQuery = locationsQuery.Where(x => x.ForSending == forSending);

        if (forReceiving.HasValue)
            locationsQuery = locationsQuery.Where(x => x.ForReceiving == forReceiving);

        if (postalServices is not null && postalServices.Count > 0)
            locationsQuery = locationsQuery.Where(x => postalServices.Contains(x.PostalService));

        if (locationTypes is not null && locationTypes.Count > 0)
            locationsQuery = locationsQuery.Where(x => locationTypes.Contains(x.LocationType));

        if (!string.IsNullOrWhiteSpace(query))
        {
            string parsedQuery = query.Trim().ToLower(); // do we need to split by " "

            locationsQuery = locationsQuery.Where(x =>
                x.Address.City.ToLower().Contains(parsedQuery) ||
                x.Address.StreetName.ToLower().Contains(parsedQuery) ||
                x.Address.PostalCode.ToLower().Contains(parsedQuery));
        }

        if (mapCircle is not null)
        {
            // TODO
        }

        return await locationsQuery.ToListAsync();
    }

    public async Task<List<ShipmentDto>> GetShipments(string postalService, List<ShipmentStatus> statuses)
    {
        List<string> validShipmentStatuses = statuses.ConvertAll(s => s.Code);

        var shipments = await _dbContext.ParcelLockerShipments
            .Where(x => x.PostalService == postalService && validShipmentStatuses.Contains(x.Status.Code))
            .Select(x => new ShipmentDto
            {
                Barcode = x.Barcode,
                PhoneNumber = x.Reciever.PhoneNumber ?? string.Empty,
                Amount = x.Amount,
                ExpirationDate = x.ExpirationDate,
            }).ToListAsync();

        return shipments ?? [];
    }

    public void SaveShipment(ParcelLockerShipment shipment)
    {
        shipment.ResetStatus();

        _dbContext.ParcelLockerShipments.Add(shipment);
    }
}
