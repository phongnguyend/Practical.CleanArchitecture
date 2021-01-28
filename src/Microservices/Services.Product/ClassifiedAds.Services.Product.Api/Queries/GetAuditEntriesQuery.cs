using ClassifiedAds.Application;
using ClassifiedAds.Infrastructure.Grpc;
using ClassifiedAds.Services.AuditLog.Grpc;
using ClassifiedAds.Services.Product.DTOs;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static ClassifiedAds.Services.AuditLog.Grpc.AuditLog;

namespace ClassifiedAds.Services.Product.Queries
{
    public class GetAuditEntriesQuery : AuditLogEntryQueryOptions, IQuery<List<AuditLogEntryDTO>>
    {
    }

    public class GetAuditEntriesQueryHandler : IQueryHandler<GetAuditEntriesQuery, List<AuditLogEntryDTO>>
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetAuditEntriesQueryHandler(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<AuditLogEntryDTO>> HandleAsync(GetAuditEntriesQuery queryOptions, CancellationToken cancellationToken = default)
        {
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var headers = new Metadata
            {
                { "Authorization", $"Bearer {token}" },
            };

            var client = new AuditLogClient(ChannelFactory.Create(_configuration["Services:AuditLog:Grpc"]));
            var entries = client.GetAuditLogEntries(new GetAuditLogEntriesRequest
            {
                ObjectId = queryOptions.ObjectId,
            }, headers);

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
