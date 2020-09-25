namespace ClassifiedAds.Application.Decorators.DatabaseRetry
{
    [Mapping(Type = typeof(DatabaseRetryAttribute))]
    public class DatabaseRetryQueryDecorator<TQuery, TResult> : DatabaseRetryDecoratorBase, IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _handler;

        public DatabaseRetryQueryDecorator(IQueryHandler<TQuery, TResult> handler, DatabaseRetryAttribute options)
        {
            DatabaseRetryOptions = options;
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