using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Embeddings;
using System;
using System.ClientModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Background.Services;

public class EmbeddingService
{
    private readonly IConfiguration _configuration;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingClient;

    public EmbeddingService(IConfiguration configuration)
    {
        _configuration = configuration;

        var options = new OpenAIOptions();
        _configuration.GetSection("Embedding:OpenAI").Bind(options);
        _embeddingClient = options.CreateEmbeddingClient();
    }

    public async Task<EmbeddingResult> GenerateAsync(string input, CancellationToken cancelationToken)
    {
        var result = await _embeddingClient.GenerateAsync([input], cancellationToken: cancelationToken);

        return new EmbeddingResult
        {
            EmbeddingVector = result.First().Vector,
            UsageDetails = result.Usage
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

        return new ChatClient(ModelId, new ApiKeyCredential(ApiKey), options).AsIChatClient();
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
    public ReadOnlyMemory<float> EmbeddingVector { get; set; }

    public UsageDetails UsageDetails { get; set; }
}
