using ClassifiedAds.Application.FileEntries.DTOs;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.BackgroundServer.HostedServices;

internal sealed class MessageBusReceiver : BackgroundService
{
    private static readonly HttpClient _httpClient = new HttpClient();

    private readonly ILogger<MessageBusReceiver> _logger;
    private readonly IConfiguration _configuration;
    private readonly IMessageReceiver<WebhookConsumer, FileUploadedEvent> _fileUploadedEventMessageReceiver;
    private readonly IMessageReceiver<WebhookConsumer, FileDeletedEvent> _fileDeletedEventMessageReceiver;

    public MessageBusReceiver(ILogger<MessageBusReceiver> logger,
        IConfiguration configuration,
        IMessageReceiver<WebhookConsumer, FileUploadedEvent> fileUploadedEventMessageReceiver,
        IMessageReceiver<WebhookConsumer, FileDeletedEvent> fileDeletedEventMessageReceiver)
    {
        _logger = logger;
        _configuration = configuration;
        _fileUploadedEventMessageReceiver = fileUploadedEventMessageReceiver;
        _fileDeletedEventMessageReceiver = fileDeletedEventMessageReceiver;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _fileUploadedEventMessageReceiver?.ReceiveAsync(async (data, metaData) =>
        {
            var url = _configuration["Webhooks:FileUploadedEvent:PayloadUrl"];
            await _httpClient.PostAsJsonAsync(url, data.FileEntry);
        }, stoppingToken);

        _fileDeletedEventMessageReceiver?.ReceiveAsync(async (data, metaData) =>
        {
            var url = _configuration["Webhooks:FileDeletedEvent:PayloadUrl"];
            await _httpClient.PostAsJsonAsync(url, data.FileEntry);
        }, stoppingToken);

        return Task.CompletedTask;
    }
}

public sealed class WebhookConsumer
{
}
