using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.ParcelLockerAggregate.Events;
using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.ParcelLockerFeatures.EventHandlers;

public class ShipmentReturnedEventHandler : INotificationHandler<ShipmentReturnedEvent>
{
    private readonly ILogger<ShipmentReturnedEventHandler> _logger;
    private readonly IPostalServiceProvider _postalServiceProvider;

    public ShipmentReturnedEventHandler(
        ILogger<ShipmentReturnedEventHandler> logger,
        IPostalServiceProvider postalServiceProvider)
    {
        _logger = logger;
        _postalServiceProvider = postalServiceProvider;
    }

    public async Task Handle(ShipmentReturnedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            bool updated = await _postalServiceProvider.UpdateShipmentStatus(notification.Shipment, ShipmentStatus.Returned);

            if (!updated)
                _logger.LogError("Failed to update status to {StatusCode} for shipment: {ShipmentId}",
                    ShipmentStatus.Returned.Code, notification.Shipment.Barcode);
            else _logger.LogDebug("{EventKey} handled.", nameof(ShipmentReturnedEvent));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling {EventKey} for shipment: {ShipmentId}", nameof(ShipmentReturnedEvent), notification.Shipment.Barcode);
        }
    }
}
