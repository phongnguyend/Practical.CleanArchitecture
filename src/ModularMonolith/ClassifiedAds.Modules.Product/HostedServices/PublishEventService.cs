using ClassifiedAds.CrossCuttingConcerns.OS;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Modules.AuditLog.Contracts.Services;
using ClassifiedAds.Modules.Product.Entities;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Product.HostedServices
{
    public class PublishEventService
    {
        private readonly ILogger<PublishEventService> _logger;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IRepository<EventLog, long> _eventLogRepository;
        private readonly IAuditLogService _externalAuditLogService;

        public PublishEventService(ILogger<PublishEventService> logger,
            IDateTimeProvider dateTimeProvider,
            IRepository<EventLog, long> eventLogRepository,
            IAuditLogService externalAuditLogService)
        {
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
            _eventLogRepository = eventLogRepository;
            _externalAuditLogService = externalAuditLogService;
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
                    var logEntry = JsonSerializer.Deserialize<AuditLog.Contracts.DTOs.AuditLogEntryDTO>(eventLog.Message);
                    await _externalAuditLogService.AddAsync(logEntry);
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
