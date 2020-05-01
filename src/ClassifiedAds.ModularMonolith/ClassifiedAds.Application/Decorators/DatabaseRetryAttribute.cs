using System;
using ClassifiedAds.Application.Decorators.DatabaseRetry;

namespace ClassifiedAds.Application.Decorators
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class DatabaseRetryAttribute : Attribute, IDatabaseRetrySettingsProvider
    {
        public int RetryTimes { get; }

        public DatabaseRetryAttribute(int retryTimes = 3)
        {
            RetryTimes = retryTimes;
        }
    }
}
