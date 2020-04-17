using System.Collections.Generic;

namespace ClassifiedAds.Domain.ValueObjects
{
    public class Money : ValueObject
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

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}
