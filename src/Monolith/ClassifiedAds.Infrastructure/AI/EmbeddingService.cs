using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Embeddings;
using System;
using System.ClientModel;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.AI;

public class EmbeddingService
{
    private readonly IConfiguration _configuration;
    private readonly OpenAIOptions _options;
    private readonly IRepository<EmbeddingCacheEntry, Guid> _embeddingCacheEntryRepository;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingClient;

    public EmbeddingService(IConfiguration configuration,
        IRepository<EmbeddingCacheEntry, Guid> embeddingCacheEntryRepository)
    {
        _configuration = configuration;
        _embeddingCacheEntryRepository = embeddingCacheEntryRepository;

        _options = new OpenAIOptions();
        _configuration.GetSection("Embedding:OpenAI").Bind(_options);
        _embeddingClient = _options.CreateEmbeddingClient();
    }

    public async Task<EmbeddingResult> GenerateAsync(string input, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Input cannot be null or empty", nameof(input));
        }

        bool cacheable = input.Length <= 450;

        if (cacheable)
        {
            var cachedEntry = await _embeddingCacheEntryRepository.GetQueryableSet()
                .FirstOrDefaultAsync(e => e.Text == input, cancellationToken: cancellationToken);

            if (cachedEntry != null)
            {
                return new EmbeddingResult
                {
                    Vector = JsonSerializer.Deserialize<ReadOnlyMemory<float>>(cachedEntry.Embedding),
                    TokenDetails = new TokenDetails
                    {
                        InputTokenCount = cachedEntry.InputTokenCount,
                        OutputTokenCount = cachedEntry.OutputTokenCount,
                        TotalTokenCount = cachedEntry.TotalTokenCount
                    }
                };
            }
        }

        var result = await _embeddingClient.GenerateAsync([input], cancellationToken: cancellationToken);

        if (cacheable)
        {
            var newCacheEntry = new EmbeddingCacheEntry
            {
                Text = input,
                Provider = "OpenAI",
                Model = _options.EmbeddingModelId,
                Embedding = JsonSerializer.Serialize(result.First().Vector),
                InputTokenCount = result.Usage.InputTokenCount,
                OutputTokenCount = result.Usage.OutputTokenCount,
                TotalTokenCount = result.Usage.TotalTokenCount
            };

            await _embeddingCacheEntryRepository.AddAsync(newCacheEntry, cancellationToken);
            await _embeddingCacheEntryRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        return new EmbeddingResult
        {
            Vector = result.First().Vector,
            TokenDetails = new TokenDetails
            {
                InputTokenCount = result.Usage.InputTokenCount,
                OutputTokenCount = result.Usage.OutputTokenCount,
                TotalTokenCount = result.Usage.TotalTokenCount
            }
        };
    }
}

public class OpenAIOptions
{
    public string Endpoint { get; set; }

    public string ApiKey { get; set; }

    public string ModelId { get; set; }

    public string EmbeddingModelId { get; set; }

    public IChatClient CreateChatClient()
    {
        return CreateOpenAIChatClient().AsIChatClient();
    }

    public ChatClient CreateOpenAIChatClient()
    {
        if (string.IsNullOrEmpty(ApiKey))
        {
            throw new ArgumentException("API Key is required", nameof(ApiKey));
        }

        if (string.IsNullOrEmpty(Endpoint))
        {
            throw new ArgumentException("Endpoint is required", nameof(Endpoint));
        }

        if (string.IsNullOrEmpty(ModelId))
        {
            throw new ArgumentException("ModelId is required", nameof(ModelId));
        }

        var options = new OpenAIClientOptions
        {
            Endpoint = new Uri(Endpoint)
        };

        return new ChatClient(ModelId, new ApiKeyCredential(ApiKey), options);
    }

    public IEmbeddingGenerator<string, Embedding<float>> CreateEmbeddingClient()
    {
        if (string.IsNullOrEmpty(ApiKey))
        {
            throw new ArgumentException("API Key is required", nameof(ApiKey));
        }

        if (string.IsNullOrEmpty(Endpoint))
        {
            throw new ArgumentException("Endpoint is required", nameof(Endpoint));
        }

        if (string.IsNullOrEmpty(EmbeddingModelId))
        {
            throw new ArgumentException("EmbeddingModelId is required", nameof(EmbeddingModelId));
        }

        var options = new OpenAIClientOptions
        {
            Endpoint = new Uri(Endpoint)
        };

        return new EmbeddingClient(EmbeddingModelId, new ApiKeyCredential(ApiKey), options).AsIEmbeddingGenerator();
    }
}

public class EmbeddingResult
{
    public ReadOnlyMemory<float> Vector { get; set; }

    public TokenDetails TokenDetails { get; set; }
}

public class TokenDetails
{
    public long? InputTokenCount { get; set; }

    public long? OutputTokenCount { get; set; }

    public long? TotalTokenCount { get; set; }
}
