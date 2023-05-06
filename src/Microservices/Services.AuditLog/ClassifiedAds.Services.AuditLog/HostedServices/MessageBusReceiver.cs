using ClassifiedAds.Application;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Services.AuditLog.DTOs;
using ClassifiedAds.Services.AuditLog.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ClassifiedAds.Services.AuditLog.HostedServices;

internal class MessageBusReceiver : BackgroundService
{
    private readonly ILogger<MessageBusReceiver> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageReceiver<AuditLogCreatedEvent> _auditLogCreatedEventReceiver;

    public MessageBusReceiver(ILogger<MessageBusReceiver> logger,
        IServiceProvider serviceProvider,
        IMessageReceiver<AuditLogCreatedEvent> auditLogCreatedEventReceiver)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _auditLogCreatedEventReceiver = auditLogCreatedEventReceiver;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _auditLogCreatedEventReceiver?.Receive(async (data, metaData) =>
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dispatcher = scope.ServiceProvider.GetRequiredService<Dispatcher>();
                data.AuditLog.Id = Guid.Empty;
                await dispatcher.DispatchAsync(new AddOrUpdateEntityCommand<AuditLogEntry>(data.AuditLog));

                _logger.LogInformation(data.AuditLog.Action);
            }
        });

        return Task.CompletedTask;
    }
}
