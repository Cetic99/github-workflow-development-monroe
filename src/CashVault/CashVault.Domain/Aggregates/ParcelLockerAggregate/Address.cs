namespace CashVault.Domain.Aggregates.ParcelLockerAggregate;

public class Address
{
    public string Country { get; private set; }
    public string City { get; private set; }
    public string PostalCode { get; private set; }
    public string StreetName { get; private set; }
    public string StreetNumber { get; private set; }

    public Address(
        string country,
        string city,
        string postalCode,
        string streetName,
        string streetNumber)
    {
        Country = country;
        City = city;
        PostalCode = postalCode;
        StreetName = streetName;
        StreetNumber = streetNumber;
    }
}
