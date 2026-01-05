using CashVault.Application.Features.ParcelLockerFeatures.QueryModels;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using MediatR;

namespace CashVault.Application.Features.ParcelLockerFeatures.Queries;

public class GetShipmentsInLockersQuery : IRequest<List<ShipmentDto>>
{
    public string CourierId { get; init; } = string.Empty;
    public string PostalService { get; init; } = string.Empty;
}

internal sealed class GetShipmentsInLockersQueryHandler : IRequestHandler<GetShipmentsInLockersQuery, List<ShipmentDto>>
{
    private IParcelLockerRepository _parcelLockerRepository;
    private ITerminalRepository _terminalRepository;

    public GetShipmentsInLockersQueryHandler(
        IParcelLockerRepository parcelLockerRepository,
        ITerminalRepository terminalRepository)
    {
        _parcelLockerRepository = parcelLockerRepository;
        _terminalRepository = terminalRepository;
    }

    public async Task<List<ShipmentDto>> Handle(GetShipmentsInLockersQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.CourierId) || string.IsNullOrEmpty(request.PostalService))
            return [];

        IParcelLockerConfiguration? configuration = await _terminalRepository.GetParcelLockerConfigurationAsync();

        if (configuration is null || configuration.ParcelLockers.Count == 0)
            return [];

        List<ShipmentDto> shipments = await _parcelLockerRepository
            .GetShipments(request.PostalService, [ShipmentStatus.Received, ShipmentStatus.Expired]);

        foreach (var shipment in shipments)
        {
            ParcelLocker? locker = configuration.GetLocker(l => l.Shipment == shipment.Barcode);
            shipment.ParcelLockerSize = locker?.Size?.Code ?? string.Empty;
        }

        return shipments.Where(x => !string.IsNullOrEmpty(x.ParcelLockerSize)).ToList();
    }
}
