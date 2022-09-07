using ClassifiedAds.CrossCuttingConcerns.OS;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Services.Product.DTOs;
using ClassifiedAds.Services.Product.Entities;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Product.HostedServices
{
    public class PublishEventService
    {
        private readonly ILogger<PublishEventService> _logger;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IRepository<OutboxEvent, long> _outboxEventRepository;
        private readonly IMessageSender<AuditLogCreatedEvent> _auditLogCreatedEventSender;
        private readonly DaprClient _daprClient;

        public PublishEventService(ILogger<PublishEventService> logger,
            IDateTimeProvider dateTimeProvider,
            IRepository<OutboxEvent, long> outboxEventRepository,
            IMessageSender<AuditLogCreatedEvent> auditLogCreatedEventSender,
            DaprClient daprClient)
        {
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
            _outboxEventRepository = outboxEventRepository;
            _auditLogCreatedEventSender = auditLogCreatedEventSender;
            _daprClient = daprClient;
        }

        public async Task<int> PublishEvents()
        {
            var events = _outboxEventRepository.GetAll()
                .Where(x => !x.Published)
                .OrderBy(x => x.CreatedDateTime)
                .Take(50)
                .ToList();

            foreach (var eventLog in events)
            {
                if (eventLog.EventType == "AUDIT_LOG_ENTRY_CREATED")
                {
                    var logEntry = JsonSerializer.Deserialize<AuditLogEntry>(eventLog.Message);
                    await _auditLogCreatedEventSender.SendAsync(new AuditLogCreatedEvent { AuditLog = logEntry });

                    if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DAPR_HTTP_PORT")))
                    {
                        await _daprClient.PublishEventAsync("pubsub", "AuditLogCreatedEvent", new AuditLogCreatedEvent { AuditLog = logEntry });
                    }
                }
                else
                {
                    // TODO: Take Note
                }

                eventLog.Published = true;
                eventLog.UpdatedDateTime = _dateTimeProvider.OffsetNow;
                await _outboxEventRepository.UnitOfWork.SaveChangesAsync();
            }

            return events.Count;
        }
    }
}
