using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.ParcelLockerAggregate.Events;
using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.ParcelLockerFeatures.EventHandlers;

public class ShipmentReceivedEventHandler : INotificationHandler<ShipmentReceivedEvent>
{
    private readonly ILogger<ShipmentReceivedEventHandler> _logger;
    private readonly IPostalServiceProvider _postalServiceProvider;

    public ShipmentReceivedEventHandler(
        ILogger<ShipmentReceivedEventHandler> logger,
        IPostalServiceProvider postalServiceProvider)
    {
        _logger = logger;
        _postalServiceProvider = postalServiceProvider;
    }

    public async Task Handle(ShipmentReceivedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            bool updated = await _postalServiceProvider.UpdateShipmentStatus(notification.Shipment, ShipmentStatus.Received);

            if (!updated)
                _logger.LogError("Failed to update status to {StatusCode} for shipment: {ShipmentId}",
                    ShipmentStatus.Received.Code, notification.Shipment.Barcode);
            else _logger.LogDebug("{EventKey} handled.", nameof(ShipmentReceivedEvent));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling {EventKey} for shipment: {ShipmentId}", nameof(ShipmentReceivedEvent), notification.Shipment.Barcode);
        }
    }
}