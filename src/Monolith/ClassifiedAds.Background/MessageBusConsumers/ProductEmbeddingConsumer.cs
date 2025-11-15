using ClassifiedAds.Application.Products.MessageBusEvents;
using ClassifiedAds.Background.Services;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
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
    private readonly IServiceProvider _serviceProvider;

    public ProductEmbeddingConsumer(ILogger<ProductEmbeddingConsumer> logger,
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public async Task HandleAsync(ProductCreatedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling ProductCreatedEvent for ProductId: {ProductId}", data?.Product?.Id);

        await ProcessProductAsync(data.Product, cancellationToken);
    }

    public async Task HandleAsync(ProductUpdatedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling ProductUpdatedEvent for ProductId: {ProductId}", data?.Product?.Id);

        await ProcessProductAsync(data.Product, cancellationToken);
    }

    public Task HandleAsync(ProductDeletedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling ProductDeletedEvent for ProductId: {ProductId}", data?.Product?.Id);
        return Task.CompletedTask;
    }

    private async Task ProcessProductAsync(Product product, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var embeddingService = scope.ServiceProvider.GetService<EmbeddingService>();

        var embedding = await embeddingService.GenerateAsync(product.Description, cancellationToken);
        var embeddingsFolder = CreateDirectoryIfNotExist(Path.Combine(_configuration["Storage:TempFolderPath"], "Embeddings", "Products"));
        await File.WriteAllTextAsync(Path.Combine(embeddingsFolder, $"{product.Id}.json"), JsonSerializer.Serialize(embedding), cancellationToken);
    }

    private static string CreateDirectoryIfNotExist(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }
}
