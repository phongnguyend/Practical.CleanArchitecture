using ClassifiedAds.CrossCuttingConcerns.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Infrastructure.Messaging;

public class MessageBus : IMessageBus
{
    private readonly IServiceProvider _serviceProvider;
    private static List<Type> _consumers = new List<Type>();
    private static Dictionary<string, List<Type>> _outboxMessageHandlers = new();

    internal static void AddConsumers(Assembly assembly, IServiceCollection services)
    {
        var types = assembly.GetTypes()
                            .Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IMessageBusConsumer<,>)))
                            .ToList();

        foreach (var type in types)
        {
            services.AddTransient(type);
        }

        _consumers.AddRange(types);
    }

    internal static void AddOutboxMessagePublishers(Assembly assembly, IServiceCollection services)
    {
        var types = assembly.GetTypes()
                            .Where(x => x.GetInterfaces().Any(y => y == typeof(IOutboxMessagePublisher)))
                            .ToList();

        foreach (var type in types)
        {
            services.AddTransient(type);
        }

        foreach (var item in types)
        {
            var canHandlerEventTypes = (string[])item.InvokeMember(nameof(IOutboxMessagePublisher.CanHandleEventTypes), BindingFlags.InvokeMethod, null, null, null, CultureInfo.CurrentCulture);
            var eventSource = (string)item.InvokeMember(nameof(IOutboxMessagePublisher.CanHandleEventSource), BindingFlags.InvokeMethod, null, null, null, CultureInfo.CurrentCulture);

            foreach (var eventType in canHandlerEventTypes)
            {
                var key = eventSource + ":" + eventType;
                if (!_outboxMessageHandlers.ContainsKey(key))
                {
                    _outboxMessageHandlers[key] = new List<Type>();
                }

                _outboxMessageHandlers[key].Add(item);
            }
        }
    }

    public MessageBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SendAsync<T>(T message, MetaData metaData = null, CancellationToken cancellationToken = default)
        where T : IMessageBusMessage
    {
        await _serviceProvider.GetRequiredService<IMessageSender<T>>().SendAsync(message, metaData, cancellationToken);
    }

    public async Task ReceiveAsync<TConsumer, T>(Func<T, MetaData, CancellationToken, Task> action, CancellationToken cancellationToken = default)
        where T : IMessageBusMessage
    {
        await _serviceProvider.GetRequiredService<IMessageReceiver<TConsumer, T>>().ReceiveAsync(action, cancellationToken);
    }

    public async Task ReceiveAsync<TConsumer, T>(CancellationToken cancellationToken = default)
        where T : IMessageBusMessage
    {
        var logger = _serviceProvider.GetRequiredService<ILogger<IMessageReceiver<TConsumer, T>>>();

        await _serviceProvider.GetRequiredService<IMessageReceiver<TConsumer, T>>().ReceiveAsync(async (data, metaData, cancellationToken) =>
        {
            using var activity = ActivityExtensions.StartNew("HandleAsync", metaData?.ActivityId);
            using var scope = _serviceProvider.CreateScope();

            var startingTimestamp = Stopwatch.GetTimestamp();

            foreach (Type handlerType in _consumers)
            {
                bool canHandleEvent = handlerType.GetInterfaces()
                    .Any(x => x.IsGenericType
                        && x.GetGenericTypeDefinition() == typeof(IMessageBusConsumer<,>)
                        && x.GenericTypeArguments[0] == typeof(TConsumer) && x.GenericTypeArguments[1] == typeof(T));

                if (canHandleEvent)
                {
                    dynamic handler = scope.ServiceProvider.GetService(handlerType);
                    await handler.HandleAsync((dynamic)data, metaData, cancellationToken);
                }
            }

            var stop = Stopwatch.GetElapsedTime(startingTimestamp);

            logger.LogInformation("{ConsumerType} handled {MessageType} in {ElapsedMilliseconds} ms.", typeof(TConsumer).Name, typeof(T).Name, stop.TotalMilliseconds);

        }, cancellationToken);
    }

    public async Task SendAsync(PublishingOutboxMessage outbox, CancellationToken cancellationToken = default)
    {
        var key = outbox.EventSource + ":" + outbox.EventType;
        var handlerTypes = _outboxMessageHandlers.TryGetValue(key, out var value) ? value : null;

        if (handlerTypes == null)
        {
            // TODO: Take Note
            return;
        }

        foreach (var type in handlerTypes)
        {
            dynamic handler = _serviceProvider.GetService(type);
            await handler.HandleAsync(outbox, cancellationToken);
        }
    }
}

public static class MessageBusExtentions
{
    public static void AddMessageBusConsumers(this IServiceCollection services, Assembly assembly)
    {
        MessageBus.AddConsumers(assembly, services);
    }

    public static void AddOutboxMessagePublishers(this IServiceCollection services, Assembly assembly)
    {
        MessageBus.AddOutboxMessagePublishers(assembly, services);
    }

    public static void AddMessageBus(this IServiceCollection services, Assembly assembly)
    {
        services.AddTransient<IMessageBus, MessageBus>();
        services.AddMessageBusConsumers(assembly);
        services.AddOutboxMessagePublishers(assembly);
    }
}