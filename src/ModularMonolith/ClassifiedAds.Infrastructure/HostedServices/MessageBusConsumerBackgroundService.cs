using ClassifiedAds.Domain.Infrastructure.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.HostedServices;

public sealed class MessageBusConsumerBackgroundService<TConsumer, T> : BackgroundService
    where T : IMessageBusEvent
{
    private readonly ILogger<MessageBusConsumerBackgroundService<TConsumer, T>> _logger;
    private readonly IMessageBus _messageBus;

    public MessageBusConsumerBackgroundService(ILogger<MessageBusConsumerBackgroundService<TConsumer, T>> logger,
        IMessageBus messageBus)
    {
        _logger = logger;
        _messageBus = messageBus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _messageBus.ReceiveAsync<TConsumer, T>(stoppingToken);
    }
}
