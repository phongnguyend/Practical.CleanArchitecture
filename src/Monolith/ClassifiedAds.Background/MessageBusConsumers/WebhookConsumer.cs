using ClassifiedAds.Application.FileEntries.MessageBusEvents;
using ClassifiedAds.Application.Products.MessageBusEvents;
using ClassifiedAds.Domain.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Background.MessageBusConsumers;

public sealed class WebhookConsumer :
    IMessageBusConsumer<WebhookConsumer, FileCreatedEvent>,
    IMessageBusConsumer<WebhookConsumer, FileUpdatedEvent>,
    IMessageBusConsumer<WebhookConsumer, FileDeletedEvent>,
    IMessageBusConsumer<WebhookConsumer, ProductCreatedEvent>,
    IMessageBusConsumer<WebhookConsumer, ProductUpdatedEvent>,
    IMessageBusConsumer<WebhookConsumer, ProductDeletedEvent>
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

    public async Task HandleAsync(FileCreatedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        var url = _configuration["Webhooks:FileCreatedEvent:PayloadUrl"];
        await _httpClient.PostAsJsonAsync(url, data, cancellationToken: cancellationToken);
    }

    public async Task HandleAsync(FileUpdatedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        var url = _configuration["Webhooks:FileUpdatedEvent:PayloadUrl"];
        await _httpClient.PostAsJsonAsync(url, data, cancellationToken: cancellationToken);
    }

    public async Task HandleAsync(FileDeletedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        var url = _configuration["Webhooks:FileDeletedEvent:PayloadUrl"];
        await _httpClient.PostAsJsonAsync(url, data, cancellationToken: cancellationToken);
    }

    public async Task HandleAsync(ProductCreatedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        var url = _configuration["Webhooks:ProductCreatedEvent:PayloadUrl"];
        await _httpClient.PostAsJsonAsync(url, data, cancellationToken: cancellationToken);
    }

    public async Task HandleAsync(ProductUpdatedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        var url = _configuration["Webhooks:ProductUpdatedEvent:PayloadUrl"];
        await _httpClient.PostAsJsonAsync(url, data, cancellationToken: cancellationToken);
    }

    public async Task HandleAsync(ProductDeletedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        var url = _configuration["Webhooks:ProductDeletedEvent:PayloadUrl"];
        await _httpClient.PostAsJsonAsync(url, data, cancellationToken: cancellationToken);
    }
}