using ClassifiedAds.Services.Product.DTOs;
using ClassifiedAds.Services.Product.Entities;
using ClassifiedAds.Services.Product.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Product.Queries;

public class GetAuditEntriesQuery : AuditLogEntryQueryOptions, IRequest<List<AuditLogEntryDTO>>
{
}

public class GetAuditEntriesQueryHandler : IRequestHandler<GetAuditEntriesQuery, List<AuditLogEntryDTO>>
{
    private readonly ProductDbContext _dbContext;
    private readonly IMediator _dispatcher;

    public GetAuditEntriesQueryHandler(ProductDbContext dbContext, IMediator dispatcher)
    {
        _dbContext = dbContext;
        _dispatcher = dispatcher;
    }

    public async Task<List<AuditLogEntryDTO>> Handle(GetAuditEntriesQuery queryOptions, CancellationToken cancellationToken = default)
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
        var users = await _dispatcher.Send(new GetUsersQuery(), cancellationToken);

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
