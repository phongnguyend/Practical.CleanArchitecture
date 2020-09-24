using ClassifiedAds.Application;
using ClassifiedAds.Modules.AuditLog.Contracts.DTOs;
using ClassifiedAds.Modules.AuditLog.Contracts.Services;
using System.Collections.Generic;

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

        public List<AuditLogEntryDTO> Handle(GetAuditEntriesQuery query)
        {
            return _auditLogService.GetAuditLogEntries(query);
        }
    }
}
