using ClassifiedAds.Application;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Services.AuditLog.DTOs;
using ClassifiedAds.Services.AuditLog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ClassifiedAds.Services.AuditLog.MessageBusConsumers;

public sealed class AuditLogAggregationConsumer : IMessageBusConsumer<AuditLogAggregationConsumer, AuditLogCreatedEvent>
{
    private readonly ILogger<AuditLogAggregationConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;

    public AuditLogAggregationConsumer(ILogger<AuditLogAggregationConsumer> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task HandleAsync(AuditLogCreatedEvent data, MetaData metaData, CancellationToken cancellationToken = default)
    {
        var dispatcher = _serviceProvider.GetRequiredService<Dispatcher>();
        var idempotentRequestRepository = _serviceProvider.GetRequiredService<IRepository<IdempotentRequest, Guid>>();

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
