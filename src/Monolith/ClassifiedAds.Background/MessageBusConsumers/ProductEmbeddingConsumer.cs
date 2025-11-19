using ClassifiedAds.Application.Products.MessageBusEvents;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Infrastructure.Messaging;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Infrastructure.AI;
using Microsoft.Data.SqlTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
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

        await ProcessProductAsync(data.Product.Id, cancellationToken);
    }

    public async Task HandleAsync(ProductUpdatedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling ProductUpdatedEvent for ProductId: {ProductId}", data?.Product?.Id);

        await ProcessProductAsync(data.Product.Id, cancellationToken);
    }

    public Task HandleAsync(ProductDeletedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling ProductDeletedEvent for ProductId: {ProductId}", data?.Product?.Id);
        return Task.CompletedTask;
    }

    private async Task ProcessProductAsync(Guid productId, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var productRepository = scope.ServiceProvider.GetService<IRepository<Product, Guid>>();
        var productEmbeddingRepository = scope.ServiceProvider.GetService<IRepository<ProductEmbedding, Guid>>();
        var embeddingService = scope.ServiceProvider.GetService<EmbeddingService>();

        var currentProduct = productRepository.GetQueryableSet().FirstOrDefault(x => x.Id == productId);
        if (currentProduct == null)
        {
            _logger.LogWarning("Product with Id: {ProductId} not found.", productId);
            return;
        }

        var currentProductEmbedding = productEmbeddingRepository.GetQueryableSet().FirstOrDefault(x => x.ProductId == productId);

        if (currentProductEmbedding == null)
        {
            var embedding = await GenerateEmbeddingAsync(embeddingService, currentProduct, cancellationToken);

            currentProductEmbedding = new ProductEmbedding
            {
                Text = currentProduct.Description,
                ProductId = currentProduct.Id,
                Embedding = new SqlVector<float>(embedding.Vector),
                TokenDetails = JsonSerializer.Serialize(embedding.TokenDetails)
            };

            await productEmbeddingRepository.AddAsync(currentProductEmbedding, cancellationToken);
            await productEmbeddingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            if (currentProductEmbedding.Text != currentProduct.Description)
            {
                var embedding = await GenerateEmbeddingAsync(embeddingService, currentProduct, cancellationToken);

                currentProductEmbedding.Text = currentProduct.Description;
                currentProductEmbedding.Embedding = new SqlVector<float>(embedding.Vector);
                currentProductEmbedding.TokenDetails = JsonSerializer.Serialize(embedding.TokenDetails);

                await productEmbeddingRepository.UpdateAsync(currentProductEmbedding, cancellationToken);
                await productEmbeddingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }

    private static string CreateDirectoryIfNotExist(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }

    private async Task<EmbeddingResult> GenerateEmbeddingAsync(EmbeddingService embeddingService, Product product, CancellationToken cancellationToken)
    {
        var embedding = await embeddingService.GenerateAsync(product.Description, cancellationToken);
        var embeddingsFolder = CreateDirectoryIfNotExist(Path.Combine(_configuration["Storage:TempFolderPath"], "Embeddings", "Products"));
        await File.WriteAllTextAsync(Path.Combine(embeddingsFolder, $"{product.Id}.json"), JsonSerializer.Serialize(embedding), cancellationToken);
        return embedding;
    }
}
