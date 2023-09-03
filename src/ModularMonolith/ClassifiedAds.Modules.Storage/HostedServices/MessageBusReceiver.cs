using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Modules.Storage.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Storage.HostedServices;

internal class MessageBusReceiver : BackgroundService
{
    private static readonly HttpClient _httpClient = new HttpClient();

    private readonly ILogger<MessageBusReceiver> _logger;
    private readonly IConfiguration _configuration;
    private readonly IMessageBus _messageBus;

    public MessageBusReceiver(ILogger<MessageBusReceiver> logger,
        IConfiguration configuration,
        IMessageBus messageBus)
    {
        _logger = logger;
        _configuration = configuration;
        _messageBus = messageBus;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _messageBus.ReceiveAsync<WebhookConsumer, FileUploadedEvent>(async (data, metaData) =>
        {
            var url = _configuration["Modules:Storage:Webhooks:FileUploadedEvent:PayloadUrl"];
            await _httpClient.PostAsJsonAsync(url, data.FileEntry);
        }, stoppingToken);

        _messageBus.ReceiveAsync<WebhookConsumer, FileDeletedEvent>(async (data, metaData) =>
        {
            var url = _configuration["Modules:Storage:Webhooks:FileDeletedEvent:PayloadUrl"];
            await _httpClient.PostAsJsonAsync(url, data.FileEntry);
        }, stoppingToken);

        return Task.CompletedTask;
    }
}

public sealed class WebhookConsumer
{
}
