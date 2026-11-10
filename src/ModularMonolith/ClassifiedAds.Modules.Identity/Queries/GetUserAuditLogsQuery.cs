using ClassifiedAds.Application;
using ClassifiedAds.Contracts.AuditLog.DTOs;
using ClassifiedAds.Contracts.AuditLog.Services;
using ClassifiedAds.Modules.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Identity.Queries;

public class GetUserAuditLogsQuery : IQuery<List<UserAuditEntryModel>>
{
    public Guid Id { get; set; }
}

public class GetUserAuditLogsQueryHandler : IQueryHandler<GetUserAuditLogsQuery, List<UserAuditEntryModel>>
{
    private readonly IAuditLogService _auditLogService;

    public GetUserAuditLogsQueryHandler(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    public async Task<List<UserAuditEntryModel>> HandleAsync(GetUserAuditLogsQuery query, CancellationToken cancellationToken = default)
    {
        var logs = await _auditLogService.GetAuditLogEntriesAsync(new AuditLogEntryQueryOptions
        {
            ObjectId = query.Id.ToString(),
            AsNoTracking = true,
        });
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var entries = new List<UserAuditEntryModel>();
        UserAuditSnapshot previous = null;
        foreach (var log in logs.Where(x => x.ObjectId == query.Id.ToString() &&
            (x.Action == "CREATED_USER" || x.Action == "UPDATED_USER" || x.Action == "DELETED_USER"))
            .OrderBy(x => x.CreatedDateTime).ThenBy(x => x.Id))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var data = JsonSerializer.Deserialize<UserAuditSnapshot>(log.Log, options);
            if (data == null)
            {
                continue;
            }

            entries.Add(new UserAuditEntryModel
            {
                Id = log.Id,
                UserName = log.UserName,
                Action = log.Action.Replace("_USER", string.Empty),
                CreatedDateTime = log.CreatedDateTime,
                Data = data,
                HighLight = typeof(UserAuditSnapshot).GetProperties().ToDictionary(
                    property => JsonNamingPolicy.CamelCase.ConvertName(property.Name),
                    property => previous != null && !Equals(property.GetValue(data), property.GetValue(previous))),
            });
            previous = data;
        }

        entries.Reverse();
        return entries;
    }
}
