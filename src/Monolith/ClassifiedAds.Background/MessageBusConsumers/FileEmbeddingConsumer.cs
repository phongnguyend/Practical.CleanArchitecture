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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.Json;
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
        if (string.IsNullOrEmpty(fileEntry?.FileLocation))
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
            // TODO: xxx
            return;
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
