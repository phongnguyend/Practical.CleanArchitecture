using ClassifiedAds.Application.FileEntries.MessageBusEvents;
using ClassifiedAds.Background.Services;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Infrastructure.Messaging;
using ClassifiedAds.Domain.Infrastructure.Storages;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Infrastructure.AI;
using CryptographyHelper;
using CryptographyHelper.SymmetricAlgorithms;
using Microsoft.Data.SqlTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
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

    private readonly string _tempFolder;
    private readonly string _markdownFolder;
    private readonly string _imageAnalysisFolder;
    private readonly string _chunkFolder;
    private readonly string _embeddingFolder;

    public FileEmbeddingConsumer(ILogger<FileEmbeddingConsumer> logger,
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;

        _tempFolder = _configuration["Storage:TempFolderPath"];
        _markdownFolder = Path.Combine(_tempFolder, "Markdown");
        _imageAnalysisFolder = Path.Combine(_tempFolder, "ImageAnalysis");
        _chunkFolder = Path.Combine(_tempFolder, "Chunks");
        _embeddingFolder = Path.Combine(_tempFolder, "Embeddings");
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

        if (fileEntry.Deleted)
        {
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var fileStorageManager = scope.ServiceProvider.GetService<IFileStorageManager>();
        var markdownService = scope.ServiceProvider.GetService<MarkdownService>();
        var imageAnalysisService = scope.ServiceProvider.GetService<ImageAnalysisService>();
        var embeddingService = scope.ServiceProvider.GetService<EmbeddingService>();
        var fileEntryTextRepository = scope.ServiceProvider.GetService<IRepository<FileEntryText, Guid>>();
        var fileEntryEmbeddingRepository = scope.ServiceProvider.GetService<IRepository<FileEntryEmbedding, Guid>>();

        var fileExtension = Path.GetExtension(fileEntry.FileName).ToLowerInvariant();

        if (fileExtension == ".txt" ||
           fileExtension == ".md" ||
           fileExtension == ".markdown")
        {

            var hasFileEntryEmbeddings = fileEntryEmbeddingRepository.GetQueryableSet().Any(x => x.FileEntryId == fileEntry.Id);

            if (hasFileEntryEmbeddings)
            {
                return;
            }

            var bytes = await GetBytesAsync(fileStorageManager, fileEntry, cancellationToken);

            var chunks = TextChunkingService.ChunkSentences(Encoding.UTF8.GetString(bytes));

            var chunksFolder = CreateDirectoryIfNotExist(Path.Combine(_chunkFolder, fileEntry.Id.ToString()));

            var embeddingsFolder = CreateDirectoryIfNotExist(Path.Combine(_embeddingFolder, fileEntry.Id.ToString()));

            foreach (var chunk in chunks)
            {
                await File.WriteAllTextAsync(Path.Combine(chunksFolder, $"{chunk.StartIndex}_{chunk.EndIndex}.txt"), chunk.Text, cancellationToken);

                var embedding = await embeddingService.GenerateAsync(chunk.Text, cancellationToken);
                await File.WriteAllTextAsync(Path.Combine(embeddingsFolder, $"{chunk.StartIndex}_{chunk.EndIndex}.json"), JsonSerializer.Serialize(embedding), cancellationToken);

                var fileEntryEmbedding = new FileEntryEmbedding
                {
                    ChunkName = $"{chunk.StartIndex}_{chunk.EndIndex}.txt",
                    ChunkLocation = Path.Combine("Chunks", fileEntry.Id.ToString(), $"{chunk.StartIndex}_{chunk.EndIndex}.txt"),
                    ShortText = Left(chunk.Text, 100),
                    FileEntryId = fileEntry.Id,
                    Embedding = new SqlVector<float>(embedding.Vector),
                    TokenDetails = JsonSerializer.Serialize(embedding.TokenDetails)
                };

                await fileEntryEmbeddingRepository.AddAsync(fileEntryEmbedding, cancellationToken);
            }

            await fileEntryEmbeddingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        else if (fileExtension == ".pdf" ||
            fileExtension == ".docx" ||
            fileExtension == ".pptx")
        {
            var fileEntryText = fileEntryTextRepository.GetQueryableSet().FirstOrDefault(x => x.FileEntryId == fileEntry.Id);

            if (fileEntryText == null)
            {
                _logger.LogInformation("Converting file to markdown for FileEntry Id: {FileEntryId}", fileEntry?.Id);

                var markdownFolder = CreateDirectoryIfNotExist(_markdownFolder);

                var markdownFile = Path.Combine(markdownFolder, $"{fileEntry.Id}.md");

                var bytes = await GetBytesAsync(fileStorageManager, fileEntry, cancellationToken);
                var markdown = await markdownService.ConvertToMarkdownAsync(bytes, fileEntry.FileName, cancellationToken);
                await File.WriteAllTextAsync(markdownFile, markdown, cancellationToken);

                fileEntryText = new FileEntryText
                {
                    TextLocation = Path.Combine("Markdown", $"{fileEntry.Id}.md"),
                    FileEntryId = fileEntry.Id,
                };

                await fileEntryTextRepository.AddAsync(fileEntryText, cancellationToken);
                await fileEntryTextRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            }

            var hasFileEntryEmbeddings = fileEntryEmbeddingRepository.GetQueryableSet().Any(x => x.FileEntryId == fileEntry.Id);

            if (!hasFileEntryEmbeddings)
            {
                var markdownFile = Path.Combine(_markdownFolder, $"{fileEntry.Id}.md");

                var chunks = TextChunkingService.ChunkSentences(await File.ReadAllTextAsync(markdownFile, cancellationToken));

                var chunksFolder = CreateDirectoryIfNotExist(Path.Combine(_chunkFolder, fileEntry.Id.ToString()));

                var embeddingsFolder = CreateDirectoryIfNotExist(Path.Combine(_embeddingFolder, fileEntry.Id.ToString()));

                foreach (var chunk in chunks)
                {
                    await File.WriteAllTextAsync(Path.Combine(chunksFolder, $"{chunk.StartIndex}_{chunk.EndIndex}.txt"), chunk.Text, cancellationToken);

                    var embedding = await embeddingService.GenerateAsync(chunk.Text, cancellationToken);
                    await File.WriteAllTextAsync(Path.Combine(embeddingsFolder, $"{chunk.StartIndex}_{chunk.EndIndex}.json"), JsonSerializer.Serialize(embedding), cancellationToken);

                    var fileEntryEmbedding = new FileEntryEmbedding
                    {
                        ChunkName = $"{chunk.StartIndex}_{chunk.EndIndex}.txt",
                        ChunkLocation = Path.Combine("Chunks", fileEntry.Id.ToString(), $"{chunk.StartIndex}_{chunk.EndIndex}.txt"),
                        ShortText = Left(chunk.Text, 100),
                        FileEntryId = fileEntry.Id,
                        Embedding = new SqlVector<float>(embedding.Vector),
                        TokenDetails = JsonSerializer.Serialize(embedding.TokenDetails)
                    };

                    await fileEntryEmbeddingRepository.AddAsync(fileEntryEmbedding, cancellationToken);
                }

                await fileEntryEmbeddingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
        else if (fileExtension == ".jpg" ||
            fileExtension == ".png")
        {
            _logger.LogInformation("Processing image file for FileEntry Id: {FileEntryId}", fileEntry?.Id);

            var imageAnalysisFolder = CreateDirectoryIfNotExist(_imageAnalysisFolder);

            var embeddingsFolder = CreateDirectoryIfNotExist(Path.Combine(_embeddingFolder, fileEntry.Id.ToString()));

            var imageAnalysisFile = Path.Combine(imageAnalysisFolder, $"{fileEntry.Id}.txt");

            var fileEntryText = fileEntryTextRepository.GetQueryableSet().FirstOrDefault(x => x.FileEntryId == fileEntry.Id);

            if (fileEntryText == null)
            {
                var bytes = await GetBytesAsync(fileStorageManager, fileEntry, cancellationToken);

                var imageAnalysisResult = await imageAnalysisService.AnalyzeImageAsync(bytes, GetMediaType(fileExtension), cancellationToken);

                var json = JsonSerializer.Serialize(imageAnalysisResult);

                await File.WriteAllTextAsync(imageAnalysisFile, json, cancellationToken);

                fileEntryText = new FileEntryText
                {
                    TextLocation = Path.Combine("ImageAnalysis", $"{fileEntry.Id}.txt"),
                    FileEntryId = fileEntry.Id,
                };

                await fileEntryTextRepository.AddAsync(fileEntryText, cancellationToken);
                await fileEntryTextRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            }

            var hasFileEntryEmbeddings = fileEntryEmbeddingRepository.GetQueryableSet().Any(x => x.FileEntryId == fileEntry.Id);

            if (!hasFileEntryEmbeddings)
            {
                var chunks = TextChunkingService.ChunkSentences(await File.ReadAllTextAsync(imageAnalysisFile, cancellationToken));

                var chunksFolder = CreateDirectoryIfNotExist(Path.Combine(_chunkFolder, fileEntry.Id.ToString()));

                foreach (var chunk in chunks)
                {
                    await File.WriteAllTextAsync(Path.Combine(chunksFolder, $"{chunk.StartIndex}_{chunk.EndIndex}.txt"), chunk.Text, cancellationToken);

                    var embedding = await embeddingService.GenerateAsync(chunk.Text, cancellationToken);
                    await File.WriteAllTextAsync(Path.Combine(embeddingsFolder, $"{chunk.StartIndex}_{chunk.EndIndex}.json"), JsonSerializer.Serialize(embedding), cancellationToken);

                    var fileEntryEmbedding = new FileEntryEmbedding
                    {
                        ChunkName = $"{chunk.StartIndex}_{chunk.EndIndex}.txt",
                        ChunkLocation = Path.Combine("Chunks", fileEntry.Id.ToString(), $"{chunk.StartIndex}_{chunk.EndIndex}.txt"),
                        ShortText = Left(chunk.Text, 100),
                        FileEntryId = fileEntry.Id,
                        Embedding = new SqlVector<float>(embedding.Vector),
                        TokenDetails = JsonSerializer.Serialize(embedding.TokenDetails)
                    };

                    await fileEntryEmbeddingRepository.AddAsync(fileEntryEmbedding, cancellationToken);
                }

                await fileEntryEmbeddingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
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

    private static string CreateDirectoryIfNotExist(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }

    private static string Left(string value, int length)
    {
        length = Math.Abs(length);
        return string.IsNullOrEmpty(value) ? value : value.Substring(0, Math.Min(value.Length, length));
    }

    private static string GetMediaType(string fileExtension)
    {
        return fileExtension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream",
        };
    }
}