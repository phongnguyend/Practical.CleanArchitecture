using ClassifiedAds.Application;
using ClassifiedAds.Contracts.AuditLog.DTOs;
using ClassifiedAds.Modules.Product.Entities;
using ClassifiedAds.Modules.Product.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Product.Queries;

public class GetAuditEntriesQuery : AuditLogEntryQueryOptions, IQuery<List<AuditLogEntryDTO>>
{
}

public class GetAuditEntriesQueryHandler : IQueryHandler<GetAuditEntriesQuery, List<AuditLogEntryDTO>>
{
    private readonly ProductDbContext _dbContext;
    private readonly Dispatcher _dispatcher;

    public GetAuditEntriesQueryHandler(ProductDbContext dbContext, Dispatcher dispatcher)
    {
        _dbContext = dbContext;
        _dispatcher = dispatcher;
    }

    public async Task<List<AuditLogEntryDTO>> HandleAsync(GetAuditEntriesQuery queryOptions, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<AuditLogEntry>() as IQueryable<AuditLogEntry>;

        if (queryOptions.UserId != Guid.Empty)
        {
            query = query.Where(x => x.UserId == queryOptions.UserId);
        }

        if (!string.IsNullOrEmpty(queryOptions.ObjectId))
        {
            query = query.Where(x => x.ObjectId == queryOptions.ObjectId);
        }

        if (queryOptions.AsNoTracking)
        {
            query = query.AsNoTracking();
        }

        var auditLogs = await query.ToListAsync();
        var users = await _dispatcher.DispatchAsync(new GetUsersQuery(), cancellationToken);

        var rs = auditLogs.Join(users, x => x.UserId, y => y.Id,
            (x, y) => new AuditLogEntryDTO
            {
                Id = x.Id,
                UserId = x.UserId,
                Action = x.Action,
                ObjectId = x.ObjectId,
                Log = x.Log,
                CreatedDateTime = x.CreatedDateTime,
                UserName = y.UserName,
            });

        return rs.OrderByDescending(x => x.CreatedDateTime).ToList();
    }
}
