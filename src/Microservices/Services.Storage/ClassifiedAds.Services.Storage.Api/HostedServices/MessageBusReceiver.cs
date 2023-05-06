using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Services.Storage.DTOs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Storage.HostedServices;

internal class MessageBusReceiver : BackgroundService
{
    private readonly ILogger<MessageBusReceiver> _logger;
    private readonly IMessageReceiver<FileUploadedEvent> _fileUploadedEventMessageReceiver;
    private readonly IMessageReceiver<FileDeletedEvent> _fileDeletedEventMessageReceiver;

    public MessageBusReceiver(ILogger<MessageBusReceiver> logger,
        IMessageReceiver<FileUploadedEvent> fileUploadedEventMessageReceiver,
        IMessageReceiver<FileDeletedEvent> fileDeletedEventMessageReceiver)
    {
        _logger = logger;
        _fileUploadedEventMessageReceiver = fileUploadedEventMessageReceiver;
        _fileDeletedEventMessageReceiver = fileDeletedEventMessageReceiver;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _fileUploadedEventMessageReceiver?.Receive(async (data, metaData) =>
        {
            string message = data.FileEntry.Id.ToString();

            _logger.LogInformation(message);

            await Task.Delay(5000); // simulate long running task
        });

        _fileDeletedEventMessageReceiver?.Receive(async (data, metaData) =>
        {
            string message = data.FileEntry.Id.ToString();

            _logger.LogInformation(message);

            await Task.Delay(5000); // simulate long running task
        });

        return Task.CompletedTask;
    }
}
