using ClassifiedAds.Application;
using ClassifiedAds.Modules.AuditLog.Contracts.DTOs;
using ClassifiedAds.Modules.AuditLog.Contracts.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Storage.Queries
{
    public class GetAuditEntriesQuery : AuditLogEntryQueryOptions, IQuery<List<AuditLogEntryDTO>>
    {
    }

    public class GetAuditEntriesQueryHandler : IQueryHandler<GetAuditEntriesQuery, List<AuditLogEntryDTO>>
    {
        private readonly IAuditLogService _auditLogService;

        public GetAuditEntriesQueryHandler(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        public Task<List<AuditLogEntryDTO>> HandleAsync(GetAuditEntriesQuery query, CancellationToken cancellationToken = default)
        {
            return _auditLogService.GetAuditLogEntriesAsync(query);
        }
    }
}
