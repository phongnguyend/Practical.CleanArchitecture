using ClassifiedAds.Application;
using ClassifiedAds.Infrastructure.Grpc;
using ClassifiedAds.Services.AuditLog.Grpc;
using ClassifiedAds.Services.Storage.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using static ClassifiedAds.Services.AuditLog.Grpc.AuditLog;

namespace ClassifiedAds.Services.Storage.Queries
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
            var client = new AuditLogClient(ChannelFactory.Create("https://localhost:5002"));
            var entries = client.GetAuditLogEntries(new GetAuditLogEntriesRequest
            {
                ObjectId = queryOptions.ObjectId,
            });

            return entries.Entries.Select(x => new AuditLogEntryDTO
            {
                Id = Guid.Parse(x.Id),
                ObjectId = x.ObjectId,
                UserId = Guid.Parse(x.UserId),
                Action = x.Action,
                Log = x.Log,
                UserName = x.UserName,
                CreatedDateTime = x.CreatedDateTime.ToDateTimeOffset(),
            }).ToList();
        }
    }
}
