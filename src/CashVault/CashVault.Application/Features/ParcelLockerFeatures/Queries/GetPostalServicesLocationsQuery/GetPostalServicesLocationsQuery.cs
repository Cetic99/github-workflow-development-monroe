using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using CashVault.Domain.Common;
using MediatR;

namespace CashVault.Application.Features.ParcelLockerFeatures.Queries;

public class GetPostalServicesLocationsQuery : IRequest<List<PostalServiceLocationDto>>
{
    public string? Query { get; set; }
    public List<string>? PostalServices { get; set; } // codes
    public List<string>? LocationTypes { get; set; }
    public bool? ForSending { get; set; } = true;
    public bool? ForReceiving { get; set; } = true;

    // TODO
    public double? CircleLongitude { get; set; }
    public double? CircleLatitude { get; set; }
    public decimal? CircleRadius { get; set; }
}


internal sealed class GetPostalServicesLocationsQueryHandler : IRequestHandler<GetPostalServicesLocationsQuery, List<PostalServiceLocationDto>>
{
    private readonly IParcelLockerRepository _parcelLockerDb;
    private readonly ITerminalRepository _terminalDb;

    public GetPostalServicesLocationsQueryHandler(
        IParcelLockerRepository parcelLockerRepository,
        ITerminalRepository terminalRepository)
    {
        _parcelLockerDb = parcelLockerRepository;
        _terminalDb = terminalRepository;
    }

    public async Task<List<PostalServiceLocationDto>> Handle(GetPostalServicesLocationsQuery request, CancellationToken cancellationToken)
    {
        var availablePostalServices = await _terminalDb.GetAvailablePostalServicesConfigurationAsync();

        IEnumerable<string>? postalServicesFilter = availablePostalServices?.AvailablePostalServices?.Select(x => x.Code);
        IEnumerable<PostalServiceLocationType>? locationTypesFilter = null;
        MapCircleFilter? circleFilter = null;

        if (request.PostalServices is not null && request.PostalServices.Count > 0)
            postalServicesFilter = postalServicesFilter?.Intersect(request.PostalServices);

        if (request.LocationTypes is not null && request.LocationTypes.Count > 0)
            locationTypesFilter = request.LocationTypes.Where(Enumeration.Contains<PostalServiceLocationType>)
                .Select(Enumeration.GetByCode<PostalServiceLocationType>);

        if (request.CircleLongitude.HasValue && request.CircleLatitude.HasValue &&
            request.CircleRadius.HasValue && request.CircleRadius.Value > 0)
            circleFilter = new()
            {
                Longitude = request.CircleLongitude.Value,
                Latitude = request.CircleLatitude.Value,
                Radius = request.CircleRadius.Value
            };

        List<PostalServiceLocation> results = await _parcelLockerDb.GetPostalServicesLocations(
                query: request.Query,
                postalServices: postalServicesFilter?.ToList(),
                locationTypes: locationTypesFilter?.ToList(),
                mapCircle: circleFilter,
                forSending: request.ForSending,
                forReceiving: request.ForReceiving);

        return results.Select(x => new PostalServiceLocationDto()
        {
            Uuid = x.Guid,
            PostalService = x.PostalService,
            LocationType = x.LocationType.Code,
            Longitude = x.Longitude,
            Latitude = x.Latitude,
            City = x.Address.City,
            StreetName = x.Address.StreetName,
            StreetNumber = x.Address.StreetNumber,
            PostalCode = x.Address.PostalCode,
            Country = x.Address.Country
        }).ToList();
    }
}