using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.ParcelLockerAggregate.Events;
using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.ParcelLockerFeatures.EventHandlers;

public class ShipmentExpiredEventHandler : INotificationHandler<ShipmentExpiredEvent>
{
    private readonly ILogger<ShipmentExpiredEventHandler> _logger;
    private readonly IPostalServiceProvider _postalServiceProvider;

    public ShipmentExpiredEventHandler(
        ILogger<ShipmentExpiredEventHandler> logger,
        IPostalServiceProvider postalServiceProvider)
    {
        _logger = logger;
        _postalServiceProvider = postalServiceProvider;
    }

    public async Task Handle(ShipmentExpiredEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            bool updated = await _postalServiceProvider.UpdateShipmentStatus(notification.Shipment, ShipmentStatus.Expired);

            if (!updated)
                _logger.LogError("Failed to update status to {StatusCode} for shipment: {ShipmentId}",
                    ShipmentStatus.Expired.Code, notification.Shipment.Barcode);
            else _logger.LogDebug("{EventKey} handled.", nameof(ShipmentExpiredEvent));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling {EventKey} for shipment: {ShipmentId}", nameof(ShipmentExpiredEvent), notification.Shipment.Barcode);
        }
    }
}