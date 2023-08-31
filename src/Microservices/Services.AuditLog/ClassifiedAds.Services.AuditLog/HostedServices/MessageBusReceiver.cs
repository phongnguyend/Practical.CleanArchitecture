using ClassifiedAds.Application;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Services.AuditLog.DTOs;
using ClassifiedAds.Services.AuditLog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ClassifiedAds.Services.AuditLog.HostedServices;

internal class MessageBusReceiver : BackgroundService
{
    private readonly ILogger<MessageBusReceiver> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageReceiver<AuditLogAggregationConsumer, AuditLogCreatedEvent> _auditLogCreatedEventReceiver;

    public MessageBusReceiver(ILogger<MessageBusReceiver> logger,
        IServiceProvider serviceProvider,
        IMessageReceiver<AuditLogAggregationConsumer, AuditLogCreatedEvent> auditLogCreatedEventReceiver)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _auditLogCreatedEventReceiver = auditLogCreatedEventReceiver;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _auditLogCreatedEventReceiver?.ReceiveAsync(async (data, metaData) =>
        {
            using var scope = _serviceProvider.CreateScope();
            await ProcessMessage(scope, data, metaData);
        }, stoppingToken);

        return Task.CompletedTask;
    }

    private async Task ProcessMessage(IServiceScope scope, AuditLogCreatedEvent data, MetaData metaData)
    {
        var dispatcher = scope.ServiceProvider.GetRequiredService<Dispatcher>();
        var idempotentRequestRepository = scope.ServiceProvider.GetRequiredService<IRepository<IdempotentRequest, Guid>>();

        var requestType = "ADD_AUDIT_LOG_ENTRY";

        var requestProcessed = await idempotentRequestRepository.GetQueryableSet().AnyAsync(x => x.RequestType == requestType && x.RequestId == metaData.MessageId);

        if (requestProcessed)
        {
            return;
        }

        var uow = idempotentRequestRepository.UnitOfWork;

        await uow.BeginTransactionAsync();

        data.AuditLog.Id = Guid.Empty;
        await dispatcher.DispatchAsync(new AddOrUpdateEntityCommand<AuditLogEntry>(data.AuditLog));

        _logger.LogInformation(data.AuditLog.Action);

        await idempotentRequestRepository.AddAsync(new IdempotentRequest
        {
            RequestType = requestType,
            RequestId = metaData.MessageId,
        });

        await uow.SaveChangesAsync();

        await uow.CommitTransactionAsync();
    }
}

public sealed class AuditLogAggregationConsumer
{
}
