using ClassifiedAds.Application;
using ClassifiedAds.Modules.AuditLog.Contracts.DTOs;
using System.Collections.Generic;

namespace ClassifiedAds.Modules.AuditLog.Contracts.Queries
{
    public class GetAuditEntriesQuery : AuditLogEntryQueryOptions, IQuery<List<AuditLogEntryDTO>>
    {
    }
}
