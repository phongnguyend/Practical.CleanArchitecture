using ClassifiedAds.DomainServices.Infrastructure;
using System;

namespace ClassifiedAds.Infrastructure
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
}
