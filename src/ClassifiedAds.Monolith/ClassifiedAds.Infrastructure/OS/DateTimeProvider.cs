using ClassifiedAds.CrossCuttingConcerns.OS;
using System;

namespace ClassifiedAds.Infrastructure.OS
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;

        public DateTimeOffset OffsetNow => DateTimeOffset.Now;
    }
}
