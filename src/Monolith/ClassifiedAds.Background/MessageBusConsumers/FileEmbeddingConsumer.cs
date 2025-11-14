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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
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

        if (string.IsNullOrEmpty(data?.FileEntry?.FileLocation))
        {
            return;
        }

        if (data.FileEntry.FileName.EndsWith(".txt") ||
           data.FileEntry.FileName.EndsWith(".md") ||
           data.FileEntry.FileName.EndsWith(".markdown"))
        {
            _logger.LogInformation("Skipping text file for FileEntry Id: {FileEntryId}", data?.FileEntry?.Id);
            return;
        }

        if (data.FileEntry.FileName.EndsWith(".pdf") ||
            data.FileEntry.FileName.EndsWith(".docx"))
        {
            _logger.LogInformation("Converting file to markdown for FileEntry Id: {FileEntryId}", data?.FileEntry?.Id);

            var markdown = await ConvertToMarkdownAsync(data.FileEntry, cancellationToken);

            return;
        }

        return;
    }

    public async Task HandleAsync(FileUpdatedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling FileUpdatedEvent for FileEntry Id: {FileEntryId}", data?.FileEntry?.Id);

        if (string.IsNullOrEmpty(data?.FileEntry?.FileLocation))
        {
            return;
        }

        if (data.FileEntry.FileName.EndsWith(".txt") ||
           data.FileEntry.FileName.EndsWith(".md") ||
           data.FileEntry.FileName.EndsWith(".markdown"))
        {
            _logger.LogInformation("Skipping text file for FileEntry Id: {FileEntryId}", data?.FileEntry?.Id);
            return;
        }

        if (data.FileEntry.FileName.EndsWith(".pdf") ||
            data.FileEntry.FileName.EndsWith(".docx"))
        {
            _logger.LogInformation("Converting file to markdown for FileEntry Id: {FileEntryId}", data?.FileEntry?.Id);

            var markdown = await ConvertToMarkdownAsync(data.FileEntry, cancellationToken);

            return;
        }

        return;
    }

    public Task HandleAsync(FileDeletedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling FileDeletedEvent for FileEntry Id: {FileEntryId}", data?.FileEntry?.Id);
        return Task.CompletedTask;
    }

    private async Task<string> ConvertToMarkdownAsync(FileEntry fileEntry, CancellationToken cancellationToken = default)
    {
        // TODO: xxx
        var content = await _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IFileStorageManager>().ReadAsync(fileEntry, cancellationToken);

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
}
