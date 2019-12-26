using System;

namespace ClassifiedAds.DomainServices.Infrastructure.OS
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}
