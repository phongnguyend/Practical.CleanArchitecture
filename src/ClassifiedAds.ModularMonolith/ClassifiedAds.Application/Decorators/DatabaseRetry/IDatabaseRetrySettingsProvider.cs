using ClassifiedAds.Application.Decorators.Core;

namespace ClassifiedAds.Application.Decorators.DatabaseRetry
{
    internal interface IDatabaseRetrySettingsProvider : ISettingsProvider
    {
        int RetryTimes { get; }
    }
}