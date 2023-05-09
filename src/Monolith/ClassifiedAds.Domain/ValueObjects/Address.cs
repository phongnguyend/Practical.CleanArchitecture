namespace ClassifiedAds.Domain.ValueObjects;

public record Address
{
    public string Street { get; private set; }

    public string City { get; private set; }

    public string ZipCode { get; private set; }

    private Address()
    {
    }

    public Address(string street, string city, string zipCode)
    {
        Street = street;
        City = city;
        ZipCode = zipCode;
    }
}
