using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Infrastructure.MessageBrokers;

public class MessageBus : IMessageBus
{
    private readonly IServiceProvider _serviceProvider;
    private static List<Type> _consumers = new List<Type>();
    private static Dictionary<string, List<Type>> _outboxEventHandlers = new();

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

    internal static void AddOutboxEventPublishers(Assembly assembly, IServiceCollection services)
    {
        var types = assembly.GetTypes()
                            .Where(x => x.GetInterfaces().Any(y => y == typeof(IOutBoxEventPublisher)))
                            .ToList();

        foreach (var type in types)
        {
            services.AddTransient(type);
        }

        foreach (var item in types)
        {
            var canHandlerEventTypes = (string[])item.InvokeMember(nameof(IOutBoxEventPublisher.CanHandleEventTypes), BindingFlags.InvokeMethod, null, null, null, CultureInfo.CurrentCulture);
            var eventSource = (string)item.InvokeMember(nameof(IOutBoxEventPublisher.CanHandleEventSource), BindingFlags.InvokeMethod, null, null, null, CultureInfo.CurrentCulture);

            foreach (var eventType in canHandlerEventTypes)
            {
                var key = eventSource + ":" + eventType;
                if (!_outboxEventHandlers.ContainsKey(key))
                {
                    _outboxEventHandlers[key] = new List<Type>();
                }

                _outboxEventHandlers[key].Add(item);
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

    public async Task ReceiveAsync<TConsumer, T>(Func<T, MetaData, Task> action, CancellationToken cancellationToken = default)
        where T : IMessageBusMessage
    {
        await _serviceProvider.GetRequiredService<IMessageReceiver<TConsumer, T>>().ReceiveAsync(action, cancellationToken);
    }

    public async Task ReceiveAsync<TConsumer, T>(CancellationToken cancellationToken = default)
        where T : IMessageBusMessage
    {
        await _serviceProvider.GetRequiredService<IMessageReceiver<TConsumer, T>>().ReceiveAsync(async (data, metaData) =>
        {
            using var scope = _serviceProvider.CreateScope();
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
        }, cancellationToken);
    }

    public async Task SendAsync(PublishingOutBoxEvent outbox, CancellationToken cancellationToken = default)
    {
        var key = outbox.EventSource + ":" + outbox.EventType;
        var handlerTypes = _outboxEventHandlers.ContainsKey(key) ? _outboxEventHandlers[key] : null;

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

    public static void AddOutboxEventPublishers(this IServiceCollection services, Assembly assembly)
    {
        MessageBus.AddOutboxEventPublishers(assembly, services);
    }

    public static void AddMessageBus(this IServiceCollection services, Assembly assembly)
    {
        services.AddTransient<IMessageBus, MessageBus>();
        services.AddMessageBusConsumers(assembly);
        services.AddOutboxEventPublishers(assembly);
    }
}