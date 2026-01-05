using CashVault.Application.Features.ParcelLockerFeatures.QueryModels;
using CashVault.Application.Interfaces;
using MediatR;

namespace CashVault.Application.Features.ParcelLockerFeatures.Queries;

public class GetPendingShipmentsQuery : IRequest<List<ShipmentDto>>
{
    public string CourierId { get; init; } = string.Empty;
    public string PostalService { get; init; } = string.Empty;
}

internal sealed class GetPendingShipmentsQueryHandler : IRequestHandler<GetPendingShipmentsQuery, List<ShipmentDto>>
{
    private IPostalServiceProvider _postalServiceProvider;

    public GetPendingShipmentsQueryHandler(
        IPostalServiceProvider postalServiceProvider)
    {
        _postalServiceProvider = postalServiceProvider;
    }

    public async Task<List<ShipmentDto>> Handle(GetPendingShipmentsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.CourierId) || string.IsNullOrEmpty(request.PostalService))
            return [];

        return await _postalServiceProvider.GetPendingShipments(request.PostalService);
    }
}