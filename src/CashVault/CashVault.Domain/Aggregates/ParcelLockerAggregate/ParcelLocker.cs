using System;
using System.Text.Json.Serialization;

namespace CashVault.Domain.Aggregates.ParcelLockerAggregate;

public class ParcelLocker
{
    public int Id { get; set; }
    public string SerialNumber { get; set; }
    public ParcelLockerSize Size { get; set; }
    public string? PostalService { get; set; }
    public string? Shipment { get; set; }
    public bool IsActive { get; set; }
    public bool HasError { get; set; }
    public string? ErrorMessage { get; set; }

    [JsonIgnore]
    public bool IsEmpty { get => string.IsNullOrWhiteSpace(Shipment); }

    public ParcelLocker(
        int id,
        string serialNumber,
        ParcelLockerSize size,
        string? postalService = null,
        string? shipment = null,
        bool isActive = true)
    {
        Id = id;
        SerialNumber = serialNumber;
        Size = size;
        PostalService = postalService;
        Shipment = shipment;
        IsActive = isActive;
    }

    public void Empty()
    {
        Shipment = null;
    }

    public void SetError(string message)
    {
        HasError = true;
        ErrorMessage = message;
    }

    public void RemoveError()
    {
        HasError = false;
        ErrorMessage = null;
    }

    public void AssignShipment(ParcelLockerShipment shipment)
    {
        if (!IsActive || !IsEmpty)
            throw new InvalidOperationException("Parcel locker is not available.");

        if (!string.IsNullOrWhiteSpace(PostalService) && shipment.PostalService != PostalService)
            throw new InvalidOperationException("Shipment postal service does not match parcel locker postal service.");

        Shipment = shipment.Barcode;
    }

}