namespace ClassifiedAds.Application.Decorators.DatabaseRetry
{
    public class DatabaseRetryQueryDecorator<TQuery, TResult> : DatabaseRetryDecoratorBase, IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _handler;

        public DatabaseRetryQueryDecorator(IQueryHandler<TQuery, TResult> handler)
        {
            _handler = handler;
        }

        public TResult Handle(TQuery query)
        {
            TResult result = default;
            WrapExecution(() => result = _handler.Handle(query));
            return result;
        }
    }
}