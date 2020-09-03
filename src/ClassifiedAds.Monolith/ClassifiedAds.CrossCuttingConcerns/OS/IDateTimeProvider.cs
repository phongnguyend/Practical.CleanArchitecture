using System;

namespace ClassifiedAds.CrossCuttingConcerns.OS
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }

        DateTimeOffset OffsetNow { get; }
    }
}
