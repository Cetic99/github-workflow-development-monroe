using CashVault.Domain.Aggregates.ParcelLockerAggregate.Events;
using CashVault.Domain.Common;
using CashVault.Domain.Common.Events;
using CashVault.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace CashVault.Domain.Aggregates.ParcelLockerAggregate;

public class ParcelLockerShipment : Entity, IAggregateRoot
{
    private List<BaseEvent> _domainEvents = [];
    private int shipmentStatusId { get; set; }

    public Guid Guid { get; private set; }
    public string Barcode { get; private set; }
    public long RegistrationNumber { get; private set; }
    public long LockerAccessCode { get; private set; }
    public ShipmentStatus Status { get; private set; }
    public bool PaymentRequired { get; private set; }
    public decimal? Amount { get; private set; }
    public Currency? Currency { get; private set; }
    public int ParcelLockerId { get; private set; }
    public string PostalService { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string? CreatedByCourierId { get; private set; }
    public string? UpdatedByCourierId { get; private set; }
    public DateTime ExpirationDate { get; private set; }
    public Address? Address { get; private set; }
    public PostalServiceLocationType? AddressLocationType { get; private set; }
    public ShipmentParty? Sender { get; private set; }
    public ShipmentParty? Reciever { get; private set; }

    public List<BaseEvent> DomainEvents => _domainEvents;

    private ParcelLockerShipment() { }

    public ParcelLockerShipment(
        string barcode,
        long registrationNumber,
        long lockerAccessCode,
        bool paymentRequired,
        decimal? amount,
        Currency? currency,
        ParcelLocker parcelLocker,
        string postalService,
        DateTime expirationDate,
        ShipmentParty? reciever,
        ShipmentStatus? status = null,
        Address? address = null,
        PostalServiceLocationType? addressLocationType = null,
        ShipmentParty? sender = null,
        string? courier = null)
    {
        Guid = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;

        Barcode = barcode;
        RegistrationNumber = registrationNumber;
        LockerAccessCode = lockerAccessCode;
        shipmentStatusId = status?.Id ?? ShipmentStatus.Draft.Id;
        Status = status ?? ShipmentStatus.Draft;
        ExpirationDate = expirationDate;
        PaymentRequired = paymentRequired;
        Amount = amount;
        Currency = currency;
        ParcelLockerId = parcelLocker.Id;
        PostalService = postalService;
        Reciever = reciever;
        Address = address;
        AddressLocationType = addressLocationType;
        Sender = sender;
        CreatedByCourierId = courier;

        Validate();
    }

    #region Status change

    public bool Accept(string? courierId = null)
    {
        if (!ShipmentStatus.Received.Equals(Status) && !ShipmentStatus.Expired.Equals(Status))
            throw new InvalidOperationException("Collected shipment cannot be accepted again.");

        if (ShipmentStatus.Received.Equals(Status))
        {
            Status = ShipmentStatus.Collected;

            AddDomainEvent(new ShipmentCollectedEvent(this));
        }
        else
        {
            if (string.IsNullOrWhiteSpace(courierId))
                throw new InvalidOperationException("Courier is required to accept expired shipments.");

            Status = ShipmentStatus.Returned;

            AddDomainEvent(new ShipmentReturnedEvent(this));
        }

        UpdatedByCourierId = courierId;
        return true;
    }

    public bool Return(string? courierId)
    {
        if (!ShipmentStatus.Expired.Equals(Status))
            throw new InvalidOperationException("Only expired shipments can be returned.");

        Status = ShipmentStatus.Returned;
        UpdatedByCourierId = courierId;

        AddDomainEvent(new ShipmentReturnedEvent(this));

        return true;
    }

    public bool Recieve()
    {
        if (!ShipmentStatus.Draft.Equals(Status) && !ShipmentStatus.Announced.Equals(Status))
            throw new InvalidOperationException("Only draft and announced shipments can be registered.");

        Status = ShipmentStatus.Received;
        AddDomainEvent(new ShipmentReceivedEvent(this));

        return true;
    }

    public bool Expire()
    {
        if (ShipmentStatus.Expired.Equals(Status) || ExpirationDate > DateTime.UtcNow)
            return false;

        Status = ShipmentStatus.Expired;
        AddDomainEvent(new ShipmentExpiredEvent(this));

        return true;
    }

    public bool CheckIfCanBePickedUp()
    {
        return ShipmentStatus.Received.Equals(Status) &&
                ExpirationDate >= DateTime.UtcNow;
    }

    public bool CheckIfCanBePickedUpByCourier()
    {
        return ShipmentStatus.Received.Equals(Status) ||
            ShipmentStatus.Expired.Equals(Status);
    }

    public void ResetStatus()
    {
        shipmentStatusId = Status.Id;
        Status = null!;
    }

    #endregion

    public void ChangeParcelLocker(ParcelLocker newLocker)
    {
        if (ParcelLockerId.Equals(newLocker.Id))
            return;

        ParcelLockerId = newLocker.Id;
    }

    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents ??= [];
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    #region Private methods

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Barcode))
            throw new ArgumentException(nameof(Barcode));

        if (RegistrationNumber <= 0)
            throw new ArgumentException(nameof(RegistrationNumber));

        if (LockerAccessCode <= 0)
            throw new ArgumentException(nameof(LockerAccessCode));

        //if (ParcelLockerId <= 0)
        //    throw new ArgumentException(nameof(ParcelLockerId));

        if (string.IsNullOrEmpty(PostalService))
            throw new ArgumentException(nameof(PostalService));

        if (shipmentStatusId <= 0)
            throw new ArgumentException(nameof(Status));

        if (PaymentRequired && (Currency is null || Amount <= 0))
            throw new ArgumentException(nameof(PaymentRequired));
    }

    #endregion
}
