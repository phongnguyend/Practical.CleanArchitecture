namespace ClassifiedAds.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; }

    public string Currency { get; }

    private Money()
    {
    }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }
}
