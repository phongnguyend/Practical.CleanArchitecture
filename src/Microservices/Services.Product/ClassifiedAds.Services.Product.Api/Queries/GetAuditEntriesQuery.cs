using ClassifiedAds.Application;
using ClassifiedAds.Infrastructure.Grpc;
using ClassifiedAds.Services.AuditLog.Grpc;
using ClassifiedAds.Services.Product.DTOs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using static ClassifiedAds.Services.AuditLog.Grpc.AuditLog;

namespace ClassifiedAds.Services.Product.Queries
{
    public class GetAuditEntriesQuery : AuditLogEntryQueryOptions, IQuery<List<AuditLogEntryDTO>>
    {
    }

    public class GetAuditEntriesQueryHandler : IQueryHandler<GetAuditEntriesQuery, List<AuditLogEntryDTO>>
    {
        private readonly IConfiguration _configuration;

        public GetAuditEntriesQueryHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<AuditLogEntryDTO> Handle(GetAuditEntriesQuery queryOptions)
        {
            var client = new AuditLogClient(ChannelFactory.Create(_configuration["Services:AuditLog:Grpc"]));
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
