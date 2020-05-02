namespace ClassifiedAds.Application.Decorators.DatabaseRetry
{
    public class DatabaseRetryCommandDecorator<TCommand> : DatabaseRetryDecoratorBase, ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _handler;

        public DatabaseRetryCommandDecorator(ICommandHandler<TCommand> handler)
        {
            _handler = handler;
        }

        public void Handle(TCommand command)
        {
            WrapExecution(() => _handler.Handle(command));
        }
    }
}
