using CashVault.Domain.Common;
using System;

namespace CashVault.Domain.Aggregates.ParcelLockerAggregate;

public class PostalServiceLocation : Entity
{
    private int postalServiceLocationTypeId { get; set; }

    public Guid Guid { get; private set; }
    public string PostalService { get; private set; } // code
    public PostalServiceLocationType LocationType { get; private set; }
    public string ContactNumber { get; private set; }
    public Address Address { get; private set; }
    public double Longitude { get; private set; }
    public double Latitude { get; private set; }
    public bool ForSending { get; private set; }
    public bool ForReceiving { get; private set; } // pick up

    public PostalServiceLocation() { }

    public PostalServiceLocation(
        string postalService,
        string contactNumber,
        Address address,
        double longitude,
        double latitude,
        PostalServiceLocationType type,
        bool forSending = true,
        bool forReceiving = true)
    {
        Guid = Guid.NewGuid();

        PostalService = postalService;
        ContactNumber = contactNumber;
        Longitude = longitude;
        Latitude = latitude;
        LocationType = type;
        Address = address;
        ForSending = forSending;
        ForReceiving = forReceiving;
    }
}
