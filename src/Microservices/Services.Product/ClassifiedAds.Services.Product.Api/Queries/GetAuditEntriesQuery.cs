using ClassifiedAds.Application;
using ClassifiedAds.Services.AuditLog.Contracts.DTOs;
using System.Collections.Generic;

namespace ClassifiedAds.Services.AuditLog.Contracts.Queries
{
    public class GetAuditEntriesQuery : AuditLogEntryQueryOptions, IQuery<List<AuditLogEntryDTO>>
    {
    }

    public class GetAuditEntriesQueryHandler : IQueryHandler<GetAuditEntriesQuery, List<AuditLogEntryDTO>>
    {
        public GetAuditEntriesQueryHandler()
        {
        }

        public List<AuditLogEntryDTO> Handle(GetAuditEntriesQuery queryOptions)
        {
            // TODO: xxx
            return new List<AuditLogEntryDTO>();
        }
    }
}
