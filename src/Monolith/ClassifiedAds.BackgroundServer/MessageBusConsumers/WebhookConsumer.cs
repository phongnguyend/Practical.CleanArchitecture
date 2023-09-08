using ClassifiedAds.Application.FileEntries.DTOs;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.BackgroundServer.MessageBusConsumers;

public sealed class WebhookConsumer :
    IMessageBusConsumer<WebhookConsumer, FileUploadedEvent>,
    IMessageBusConsumer<WebhookConsumer, FileDeletedEvent>
{
    private static readonly HttpClient _httpClient = new HttpClient();

    private readonly ILogger<WebhookConsumer> _logger;
    private readonly IConfiguration _configuration;

    public WebhookConsumer(ILogger<WebhookConsumer> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task HandleAsync(FileUploadedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        var url = _configuration["Webhooks:FileUploadedEvent:PayloadUrl"];
        await _httpClient.PostAsJsonAsync(url, data.FileEntry, cancellationToken: cancellationToken);
    }

    public async Task HandleAsync(FileDeletedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        var url = _configuration["Webhooks:FileDeletedEvent:PayloadUrl"];
        await _httpClient.PostAsJsonAsync(url, data.FileEntry, cancellationToken: cancellationToken);
    }
}