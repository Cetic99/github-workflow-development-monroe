using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using MediatR;

namespace CashVault.Application.Features.ParcelLockerFeatures.Queries;

public record GetPostalServicesQuery : IRequest<List<PostalServiceDto>>
{ }

internal sealed class GetPostalServicesQueryHandler : IRequestHandler<GetPostalServicesQuery, List<PostalServiceDto>>
{
    private readonly ITerminalRepository _db;

    public GetPostalServicesQueryHandler(
        ITerminalRepository db)
    {
        _db = db;
    }

    public async Task<List<PostalServiceDto>> Handle(GetPostalServicesQuery request, CancellationToken cancellationToken)
    {
        PostalServicesConfiguration? postalServicesConfig = await _db.GetPostalServicesConfigurationAsync();

        if (postalServicesConfig is null || postalServicesConfig.PostalServices.Count < 1)
            return [];

        AvailablePostalServicesConfiguration? availablePostalServicesConfig = await _db.GetAvailablePostalServicesConfigurationAsync();

        if (availablePostalServicesConfig is null || availablePostalServicesConfig.AvailablePostalServices.Count < 1)
            return [];

        var availableCodes = availablePostalServicesConfig.AvailablePostalServices
            .Select(x => x.Code)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return postalServicesConfig.PostalServices
                .Where(x => availableCodes.Contains(x.Code))
                .OrderBy(x => x.DisplaySequence)
                .Select(x => new PostalServiceDto()
                {
                    Code = x.Code,
                    Name = x.Name
                }).ToList();
    }
}
