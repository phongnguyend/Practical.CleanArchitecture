using System;

namespace ClassifiedAds.Application.Decorators
{
    public abstract class DatabaseRetryDecoratorBase
    {
        private const int RetryTimes = 3;

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
                    if (executedTimes >= RetryTimes || !IsDatabaseException(ex))
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