using ClassifiedAds.CrossCuttingConcerns.OS;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Services.Product.DTOs;
using ClassifiedAds.Services.Product.Entities;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Product.HostedServices
{
    public class PublishEventService
    {
        private readonly ILogger<PublishEventService> _logger;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IRepository<EventLog, long> _eventLogRepository;
        private readonly IMessageSender<AuditLogCreatedEvent> _auditLogCreatedEventSender;

        public PublishEventService(ILogger<PublishEventService> logger,
            IDateTimeProvider dateTimeProvider,
            IRepository<EventLog, long> eventLogRepository,
            IMessageSender<AuditLogCreatedEvent> auditLogCreatedEventSender)
        {
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
            _eventLogRepository = eventLogRepository;
            _auditLogCreatedEventSender = auditLogCreatedEventSender;
        }

        public async Task<int> PublishEvents()
        {
            var events = _eventLogRepository.GetAll()
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
                }
                else
                {
                    // TODO: Take Note
                }

                eventLog.Published = true;
                eventLog.UpdatedDateTime = _dateTimeProvider.OffsetNow;
                await _eventLogRepository.UnitOfWork.SaveChangesAsync();
            }

            return events.Count;
        }
    }
}
