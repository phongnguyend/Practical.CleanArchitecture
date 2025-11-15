using Azure;
using Azure.AI.Vision.ImageAnalysis;
using ClassifiedAds.Application.FileEntries.MessageBusEvents;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Infrastructure.Messaging;
using ClassifiedAds.Domain.Infrastructure.Storages;
using CryptographyHelper;
using CryptographyHelper.SymmetricAlgorithms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Background.MessageBusConsumers;

public sealed class FileEmbeddingConsumer :
    IMessageBusConsumer<FileEmbeddingConsumer, FileCreatedEvent>,
    IMessageBusConsumer<FileEmbeddingConsumer, FileUpdatedEvent>,
    IMessageBusConsumer<FileEmbeddingConsumer, FileDeletedEvent>
{
    private static readonly HttpClient _httpClient = new HttpClient();

    private readonly ILogger<FileEmbeddingConsumer> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public FileEmbeddingConsumer(ILogger<FileEmbeddingConsumer> logger,
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public async Task HandleAsync(FileCreatedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling FileCreatedEvent for FileEntry Id: {FileEntryId}", data?.FileEntry?.Id);

        await ProcessFileAsync(data.FileEntry, cancellationToken);
    }

    public async Task HandleAsync(FileUpdatedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling FileUpdatedEvent for FileEntry Id: {FileEntryId}", data?.FileEntry?.Id);

        await ProcessFileAsync(data.FileEntry, cancellationToken);
    }

    public Task HandleAsync(FileDeletedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling FileDeletedEvent for FileEntry Id: {FileEntryId}", data?.FileEntry?.Id);

        return Task.CompletedTask;
    }

    private async Task ProcessFileAsync(FileEntry fileEntry, CancellationToken cancellationToken)
    {
        if (fileEntry == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(fileEntry.FileLocation))
        {
            return;
        }

        if (fileEntry.Encrypted && string.IsNullOrEmpty(fileEntry.EncryptionKey))
        {
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var fileStorageManager = scope.ServiceProvider.GetService<IFileStorageManager>();

        var fileExtension = Path.GetExtension(fileEntry.FileName);

        if (fileExtension == ".txt" ||
           fileExtension == ".md" ||
           fileExtension == ".markdown")
        {

            var bytes = await GetBytesAsync(fileStorageManager, fileEntry, cancellationToken);

            var chunks = TextChunkingService.ChunkSentences(Encoding.UTF8.GetString(bytes));

            var chunksFolder = Path.Combine(_configuration["Storage:TempFolderPath"], "Chunks", fileEntry.Id.ToString());

            if (!Directory.Exists(chunksFolder))
            {
                Directory.CreateDirectory(chunksFolder);
            }

            foreach (var chunk in chunks)
            {
                await File.WriteAllTextAsync(Path.Combine(chunksFolder, $"{chunk.StartIndex}_{chunk.EndIndex}.txt"), chunk.Text, cancellationToken);
            }
        }
        else if (fileExtension == ".pdf" ||
            fileExtension == ".docx" ||
            fileExtension == ".pptx")
        {
            _logger.LogInformation("Converting file to markdown for FileEntry Id: {FileEntryId}", fileEntry?.Id);

            var markdownFolder = Path.Combine(_configuration["Storage:TempFolderPath"], "Markdown");

            if (!Directory.Exists(markdownFolder))
            {
                Directory.CreateDirectory(markdownFolder);
            }

            var markdownFile = Path.Combine(markdownFolder, fileEntry.Id + ".md");

            if (!File.Exists(markdownFile))
            {
                var markdown = await ConvertToMarkdownAsync(fileStorageManager, fileEntry, cancellationToken);
                await File.WriteAllTextAsync(markdownFile, markdown, cancellationToken);
            }

            var chunks = TextChunkingService.ChunkSentences(await File.ReadAllTextAsync(markdownFile, cancellationToken));

            var chunksFolder = Path.Combine(_configuration["Storage:TempFolderPath"], "Chunks", fileEntry.Id.ToString());

            if (!Directory.Exists(chunksFolder))
            {
                Directory.CreateDirectory(chunksFolder);
            }

            foreach (var chunk in chunks)
            {
                await File.WriteAllTextAsync(Path.Combine(chunksFolder, $"{chunk.StartIndex}_{chunk.EndIndex}.txt"), chunk.Text, cancellationToken);
            }
        }
        else if (fileExtension == ".jpg" ||
            fileExtension == ".png")
        {
            _logger.LogInformation("Processing image file for FileEntry Id: {FileEntryId}", fileEntry?.Id);

            var imageAnalysisFolder = Path.Combine(_configuration["Storage:TempFolderPath"], "ImageAnalysis");

            if (!Directory.Exists(imageAnalysisFolder))
            {
                Directory.CreateDirectory(imageAnalysisFolder);
            }

            var imageAnalysisFile = Path.Combine(imageAnalysisFolder, fileEntry.Id + ".json");

            if (!File.Exists(imageAnalysisFile))
            {
                var imageAnalysisResult = await AnalyzeImageAsync(fileStorageManager, fileEntry, cancellationToken);

                await File.WriteAllTextAsync(imageAnalysisFile, JsonSerializer.Serialize(imageAnalysisResult), cancellationToken);
            }
        }
    }

    private async Task<byte[]> GetBytesAsync(IFileStorageManager fileStorageManager, FileEntry fileEntry, CancellationToken cancellationToken)
    {
        var content = await fileStorageManager.ReadAsync(fileEntry, cancellationToken);

        if (fileEntry.Encrypted)
        {
            var masterEncryptionKey = _configuration["Storage:MasterEncryptionKey"];
            var encryptionKey = fileEntry.EncryptionKey.FromBase64String()
                      .UseAES(masterEncryptionKey.FromBase64String())
                      .WithCipher(CipherMode.CBC)
                      .WithIV(fileEntry.EncryptionIV.FromBase64String())
                      .WithPadding(PaddingMode.PKCS7)
                      .Decrypt();

            content = fileEntry.FileLocation != "Fake.txt"
                ? content
                .UseAES(encryptionKey)
                .WithCipher(CipherMode.CBC)
                .WithIV(fileEntry.EncryptionIV.FromBase64String())
                .WithPadding(PaddingMode.PKCS7)
                .Decrypt()
                : content;
        }

        return content;
    }

    private async Task<string> ConvertToMarkdownAsync(IFileStorageManager fileStorageManager, FileEntry fileEntry, CancellationToken cancellationToken = default)
    {
        var content = await GetBytesAsync(fileStorageManager, fileEntry, cancellationToken);

        using var form = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(content);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        form.Add(fileContent, "formFile", fileEntry.FileName);
        form.Add(new StringContent("Test Name"), "name");

        var response = await _httpClient.PostAsync(_configuration["TextExtracting:MarkItDownServer:Endpoint"], form, cancellationToken);
        response.EnsureSuccessStatusCode();

        var markdown = await response.Content.ReadAsStringAsync(cancellationToken);

        return markdown;
    }

    private ImageAnalysisClient CreateImageAnalysisClient(string endpoint, string key)
    {
        var client = new ImageAnalysisClient(new Uri(endpoint), new AzureKeyCredential(key));
        return client;
    }

    private async Task<ImageAnalysisResult> AnalyzeImageAsync(IFileStorageManager fileStorageManager, FileEntry fileEntry, CancellationToken cancellationToken = default)
    {
        string key = _configuration["ImageAnalysis:AzureAIVision:ApiKey"]!;
        string endpoint = _configuration["ImageAnalysis:AzureAIVision:Endpoint"]!;

        // Create a client
        var client = CreateImageAnalysisClient(endpoint, key);
        var bytes = await GetBytesAsync(fileStorageManager, fileEntry, cancellationToken);

        // Creating a list that defines the features to be extracted from the image.
        VisualFeatures features = VisualFeatures.Caption | VisualFeatures.DenseCaptions | VisualFeatures.Tags;

        // Analyze the image
        var result = await client.AnalyzeAsync(new BinaryData(bytes), visualFeatures: features, cancellationToken: cancellationToken);

        return new ImageAnalysisResult
        {
            Tags = result.Value.Tags.Values.Select(x => new Tag
            {
                Name = x.Name,
                Confidence = x.Confidence
            }).ToArray(),
            Caption = new Caption
            {
                Text = result.Value.Caption.Text,
                Confidence = result.Value.Caption.Confidence
            },
            DenseCaptions = result.Value.DenseCaptions.Values.Select(x => new Caption
            {
                Text = x.Text,
                Confidence = x.Confidence
            }).ToArray()
        };
    }

    class ImageAnalysisResult
    {
        public Tag[] Tags { get; set; }

        public Caption Caption { get; set; }

        public Caption[] DenseCaptions { get; set; }
    }

    class Tag
    {
        public string Name { get; set; }

        public float Confidence { get; set; }
    }

    class Caption
    {
        public string Text { get; set; }

        public float Confidence { get; set; }
    }
}

public class Chunk
{
    public required string Text { get; init; }

    public required int StartIndex { get; init; }

    public required int EndIndex { get; init; }
}

public class TextChunkingService
{
    public static IEnumerable<Chunk> ChunkSentences(string text, int maxTokens = 800)
    {
        // Split text into sentences while preserving their original positions
        var sentenceMatches = Regex.Matches(text, @"[^\.!\?]*[\.!\?]\s*");
        var sentences = new List<(string content, int start, int end)>();

        int lastEnd = 0;
        foreach (Match match in sentenceMatches)
        {
            sentences.Add((match.Value, match.Index, match.Index + match.Length - 1));
            lastEnd = match.Index + match.Length;
        }

        // Handle any remaining text that doesn't end with sentence punctuation
        if (lastEnd < text.Length)
        {
            string remaining = text.Substring(lastEnd);
            if (!string.IsNullOrWhiteSpace(remaining))
            {
                sentences.Add((remaining, lastEnd, text.Length - 1));
            }
        }

        var current = new StringBuilder();
        int tokenCount = 0;
        int chunkStartIndex = -1;
        int chunkEndIndex = -1;

        foreach (var (content, start, end) in sentences)
        {
            int sentenceTokens = content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

            if (tokenCount + sentenceTokens > maxTokens && current.Length > 0)
            {
                yield return new Chunk
                {
                    Text = current.ToString().Trim(),
                    StartIndex = chunkStartIndex,
                    EndIndex = chunkEndIndex
                };

                current.Clear();
                tokenCount = 0;
                chunkStartIndex = -1;
            }

            if (current.Length == 0)
            {
                chunkStartIndex = start;
            }

            current.Append(content);
            tokenCount += sentenceTokens;
            chunkEndIndex = end;
        }

        if (current.Length > 0)
        {
            yield return new Chunk
            {
                Text = current.ToString().Trim(),
                StartIndex = chunkStartIndex,
                EndIndex = chunkEndIndex
            };
        }
    }

    public static IEnumerable<Chunk> ChunkSentencesOverlapping(string text, int maxTokens = 800, double overlapRatio = 0.1)
    {
        // Split text into sentences while preserving their original positions
        var sentenceMatches = Regex.Matches(text, @"[^\.!\?]*[\.!\?]\s*");
        var sentences = new List<(string content, int start, int end)>();

        int lastEnd = 0;
        foreach (Match match in sentenceMatches)
        {
            sentences.Add((match.Value.Trim(), match.Index, match.Index + match.Length - 1));
            lastEnd = match.Index + match.Length;
        }

        // Handle remaining text
        if (lastEnd < text.Length)
        {
            string remaining = text.Substring(lastEnd);
            if (!string.IsNullOrWhiteSpace(remaining))
            {
                sentences.Add((remaining.Trim(), lastEnd, text.Length - 1));
            }
        }

        var current = new StringBuilder();
        int tokenCount = 0;
        int estimatedOverlapTokens = (int)(maxTokens * overlapRatio);
        var lastChunkSentences = new List<(string content, int start, int end)>();
        int chunkStartIndex = -1;
        int chunkEndIndex = -1;

        foreach (var sentence in sentences)
        {
            int sentenceTokens = EstimateTokens(sentence.content);

            // If adding this sentence exceeds limit, yield current chunk
            if (tokenCount + sentenceTokens > maxTokens && current.Length > 0)
            {
                yield return new Chunk
                {
                    Text = current.ToString().Trim(),
                    StartIndex = chunkStartIndex,
                    EndIndex = chunkEndIndex
                };

                // Prepare overlap from previous sentences
                var overlapSentences = lastChunkSentences.TakeLast(Math.Max(1, estimatedOverlapTokens / 20)).ToList();
                if (overlapSentences.Any())
                {
                    var overlap = string.Join(" ", overlapSentences.Select(s => s.content));
                    current.Clear();
                    current.Append(overlap + " ");
                    tokenCount = EstimateTokens(overlap);
                    chunkStartIndex = overlapSentences.First().start;
                }
                else
                {
                    current.Clear();
                    tokenCount = 0;
                    chunkStartIndex = sentence.start;
                }
                lastChunkSentences.Clear();
            }

            if (current.Length == 0)
            {
                chunkStartIndex = sentence.start;
            }

            current.Append(sentence.content + " ");
            tokenCount += sentenceTokens;
            chunkEndIndex = sentence.end;
            lastChunkSentences.Add(sentence);
        }

        if (current.Length > 0)
        {
            yield return new Chunk
            {
                Text = current.ToString().Trim(),
                StartIndex = chunkStartIndex,
                EndIndex = chunkEndIndex
            };
        }
    }

    private static int EstimateTokens(string text)
    {
        // Rough heuristic: 1 token ≈ 4 chars
        return Math.Max(1, text.Length / 4);
    }
}