using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Application;

public class Dispatcher
{
    private readonly IServiceProvider _provider;

    public Dispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task DispatchAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        Type type = typeof(ICommandHandler<>);
        Type[] typeArgs = { command.GetType() };
        Type handlerType = type.MakeGenericType(typeArgs);

        dynamic handler = _provider.GetService(handlerType);
        await handler.HandleAsync((dynamic)command, cancellationToken);
    }

    public async Task<T> DispatchAsync<T>(IQuery<T> query, CancellationToken cancellationToken = default)
    {
        Type type = typeof(IQueryHandler<,>);
        Type[] typeArgs = { query.GetType(), typeof(T) };
        Type handlerType = type.MakeGenericType(typeArgs);

        dynamic handler = _provider.GetService(handlerType);
        Task<T> result = handler.HandleAsync((dynamic)query, cancellationToken);

        return await result;
    }
}