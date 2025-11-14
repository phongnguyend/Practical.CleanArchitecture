using ClassifiedAds.Application.Products.MessageBusEvents;
using ClassifiedAds.Domain.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Background.MessageBusConsumers;

public sealed class ProductEmbeddingConsumer :
    IMessageBusConsumer<ProductEmbeddingConsumer, ProductCreatedEvent>,
    IMessageBusConsumer<ProductEmbeddingConsumer, ProductUpdatedEvent>,
    IMessageBusConsumer<ProductEmbeddingConsumer, ProductDeletedEvent>
{
    private readonly ILogger<ProductEmbeddingConsumer> _logger;
    private readonly IConfiguration _configuration;

    public ProductEmbeddingConsumer(ILogger<ProductEmbeddingConsumer> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public Task HandleAsync(ProductCreatedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling ProductCreatedEvent for ProductId: {ProductId}", data?.Product?.Id);
        return Task.CompletedTask;
    }

    public Task HandleAsync(ProductUpdatedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling ProductUpdatedEvent for ProductId: {ProductId}", data?.Product?.Id);
        return Task.CompletedTask;
    }

    public Task HandleAsync(ProductDeletedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling ProductDeletedEvent for ProductId: {ProductId}", data?.Product?.Id);
        return Task.CompletedTask;
    }
}
