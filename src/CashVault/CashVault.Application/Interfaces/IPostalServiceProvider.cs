using CashVault.Application.Features.ParcelLockerFeatures.Models;
using CashVault.Application.Features.ParcelLockerFeatures.QueryModels;
using CashVault.Domain.Aggregates.ParcelLockerAggregate;

namespace CashVault.Application.Interfaces;

public interface IPostalServiceProvider
{
    Task<ShipmentModel?> FetchShipment(string postalService, string? barcode, long registrationNumber);
    Task<CourierModel?> ProcessCourierBarcode(string postalService, string barcode);
    Task<List<ShipmentDto>> GetPendingShipments(string postalService);
    Task<bool> UpdateShipmentStatus(ParcelLockerShipment shipment, ShipmentStatus status);

    // TODO
    Task<ShipmentDetailsModel?> CreateShipmentAsync(CreateShipmentModel data);
}
