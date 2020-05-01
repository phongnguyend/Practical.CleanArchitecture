using System;
using ClassifiedAds.Application.Decorators.Core;

namespace ClassifiedAds.Application.Decorators.DatabaseRetry
{
    public abstract class DatabaseRetryDecoratorBase : ISettingsAcceptable
    {
        private int _retryTimes;

        void ISettingsAcceptable.Accept(ISettingsProvider settingsProvider)
        {
            if (settingsProvider is IDatabaseRetrySettingsProvider databaseRetrySettingsProvider)
            {
                _retryTimes = databaseRetrySettingsProvider.RetryTimes;
            }
        }

        protected void WrapExecution(Action action)
        {
            int executedTimes = 0;

            while (true)
            {
                try
                {
                    executedTimes++;
                    action();
                    return;
                }
                catch (Exception ex)
                {
                    if (executedTimes >= _retryTimes || !IsDatabaseException(ex))
                    {
                        throw;
                    }
                }
            }
        }

        private static bool IsDatabaseException(Exception exception)
        {
            string message = exception.InnerException?.Message;

            if (message == null)
            {
                return false;
            }

            return message.Contains("The connection is broken and recovery is not possible")
                || message.Contains("error occurred while establishing a connection");
        }
    }
}