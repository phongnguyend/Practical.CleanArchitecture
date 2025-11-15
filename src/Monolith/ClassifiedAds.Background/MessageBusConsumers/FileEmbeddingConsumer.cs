using ClassifiedAds.Application.FileEntries.MessageBusEvents;
using ClassifiedAds.Background.Services;
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
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Background.MessageBusConsumers;

public sealed class FileEmbeddingConsumer :
    IMessageBusConsumer<FileEmbeddingConsumer, FileCreatedEvent>,
    IMessageBusConsumer<FileEmbeddingConsumer, FileUpdatedEvent>,
    IMessageBusConsumer<FileEmbeddingConsumer, FileDeletedEvent>
{
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
        var markdownService = scope.ServiceProvider.GetService<MarkdownService>();
        var imageAnalysisService = scope.ServiceProvider.GetService<ImageAnalysisService>();
        var embeddingService = scope.ServiceProvider.GetService<EmbeddingService>();

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

            var embeddingsFolder = Path.Combine(_configuration["Storage:TempFolderPath"], "Embeddings", fileEntry.Id.ToString());

            if (!Directory.Exists(embeddingsFolder))
            {
                Directory.CreateDirectory(embeddingsFolder);
            }

            foreach (var chunk in chunks)
            {
                await File.WriteAllTextAsync(Path.Combine(chunksFolder, $"{chunk.StartIndex}_{chunk.EndIndex}.txt"), chunk.Text, cancellationToken);

                var embedding = await embeddingService.GenerateAsync(chunk.Text, cancellationToken);
                await File.WriteAllTextAsync(Path.Combine(embeddingsFolder, $"{chunk.StartIndex}_{chunk.EndIndex}.json"), JsonSerializer.Serialize(embedding), cancellationToken);
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
                var bytes = await GetBytesAsync(fileStorageManager, fileEntry, cancellationToken);
                var markdown = await markdownService.ConvertToMarkdownAsync(bytes, fileEntry.FileName, cancellationToken);
                await File.WriteAllTextAsync(markdownFile, markdown, cancellationToken);
            }

            var chunks = TextChunkingService.ChunkSentences(await File.ReadAllTextAsync(markdownFile, cancellationToken));

            var chunksFolder = Path.Combine(_configuration["Storage:TempFolderPath"], "Chunks", fileEntry.Id.ToString());

            if (!Directory.Exists(chunksFolder))
            {
                Directory.CreateDirectory(chunksFolder);
            }

            var embeddingsFolder = Path.Combine(_configuration["Storage:TempFolderPath"], "Embeddings", fileEntry.Id.ToString());

            if (!Directory.Exists(embeddingsFolder))
            {
                Directory.CreateDirectory(embeddingsFolder);
            }

            foreach (var chunk in chunks)
            {
                await File.WriteAllTextAsync(Path.Combine(chunksFolder, $"{chunk.StartIndex}_{chunk.EndIndex}.txt"), chunk.Text, cancellationToken);

                var embedding = await embeddingService.GenerateAsync(chunk.Text, cancellationToken);
                await File.WriteAllTextAsync(Path.Combine(embeddingsFolder, $"{chunk.StartIndex}_{chunk.EndIndex}.json"), JsonSerializer.Serialize(embedding), cancellationToken);
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

            var embeddingsFolder = Path.Combine(_configuration["Storage:TempFolderPath"], "Embeddings", fileEntry.Id.ToString());

            if (!Directory.Exists(embeddingsFolder))
            {
                Directory.CreateDirectory(embeddingsFolder);
            }

            var imageAnalysisFile = Path.Combine(imageAnalysisFolder, fileEntry.Id + ".json");

            if (!File.Exists(imageAnalysisFile))
            {
                var bytes = await GetBytesAsync(fileStorageManager, fileEntry, cancellationToken);

                var imageAnalysisResult = await imageAnalysisService.AnalyzeImageAsync(bytes, cancellationToken);

                var json = JsonSerializer.Serialize(imageAnalysisResult);

                await File.WriteAllTextAsync(imageAnalysisFile, json, cancellationToken);

                var embedding = await embeddingService.GenerateAsync(json, cancellationToken);
                await File.WriteAllTextAsync(Path.Combine(embeddingsFolder, $"{fileEntry.Id}.json"), JsonSerializer.Serialize(embedding), cancellationToken);
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
}