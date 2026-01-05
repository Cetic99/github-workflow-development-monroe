using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using CashVault.Domain.Aggregates.ParcelLockerAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.ParcelLockerFeatures.EventHandlers;

public class ShipmentCollectedEventHandler : INotificationHandler<ShipmentCollectedEvent>
{
    private readonly ILogger<ShipmentCollectedEventHandler> _logger;
    private readonly IPostalServiceProvider _postalServiceProvider;

    public ShipmentCollectedEventHandler(
        ILogger<ShipmentCollectedEventHandler> logger,
        IPostalServiceProvider postalServiceProvider)
    {
        _logger = logger;
        _postalServiceProvider = postalServiceProvider;
    }

    public async Task Handle(ShipmentCollectedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            bool updated = await _postalServiceProvider.UpdateShipmentStatus(notification.Shipment, ShipmentStatus.Collected);

            if (!updated)
                _logger.LogError("Failed to update status to {StatusCode} for shipment: {ShipmentId}",
                    ShipmentStatus.Collected.Code, notification.Shipment.Barcode);
            else _logger.LogDebug("{EventKey} handled.", nameof(ShipmentCollectedEvent));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling {EventKey} for shipment: {ShipmentId}", nameof(ShipmentCollectedEvent), notification.Shipment.Barcode);
        }
    }
}
