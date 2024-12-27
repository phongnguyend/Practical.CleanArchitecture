using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using System;

namespace ClassifiedAds.Infrastructure.DateTimes;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;

    public DateTime UtcNow => DateTime.UtcNow;

    public DateTimeOffset OffsetNow => DateTimeOffset.Now;

    public DateTimeOffset OffsetUtcNow => DateTimeOffset.UtcNow;
}
