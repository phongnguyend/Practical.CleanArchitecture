using ClassifiedAds.Application.Commands;
using System;

namespace ClassifiedAds.Application.Decorators
{
    public class DatabaseRetryDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _handler;

        public DatabaseRetryDecorator(ICommandHandler<TCommand> handler)
        {
            _handler = handler;
        }

        public void Handle(TCommand command)
        {
            for (int i = 0; ; i++)
            {
                try
                {
                    _handler.Handle(command);
                }
                catch (Exception ex)
                {
                    if (i >= 3 || !IsDatabaseException(ex))
                    {
                        throw;
                    }
                }
            }
        }

        private bool IsDatabaseException(Exception exception)
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
