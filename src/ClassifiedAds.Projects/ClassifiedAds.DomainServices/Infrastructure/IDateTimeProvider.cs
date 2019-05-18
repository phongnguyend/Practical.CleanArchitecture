using System;

namespace ClassifiedAds.DomainServices.Infrastructure
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}
