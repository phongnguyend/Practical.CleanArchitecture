using ClassifiedAds.DomainServices.Infrastructure.OS;
using System;

namespace ClassifiedAds.Infrastructure.OS
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
}
