using ClassifiedAds.Application;
using ClassifiedAds.Modules.AuditLog.Contracts.DTOs;
using ClassifiedAds.Modules.AuditLog.Contracts.Queries;
using ClassifiedAds.Modules.AuditLog.Entities;
using ClassifiedAds.Modules.AuditLog.Repositories;
using ClassifiedAds.Modules.Identity.Contracts.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.Modules.AuditLog.Queries
{
    public class GetAuditEntriesQueryHandler : IQueryHandler<GetAuditEntriesQuery, List<AuditLogEntryDTO>>
    {
        private readonly AuditLogDbContext _dbContext;
        private readonly Dispatcher _dispatcher;

        public GetAuditEntriesQueryHandler(AuditLogDbContext dbContext, Dispatcher dispatcher)
        {
            _dbContext = dbContext;
            _dispatcher = dispatcher;
        }

        public List<AuditLogEntryDTO> Handle(GetAuditEntriesQuery queryOptions)
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

            var auditLogs = query.ToList();
            var users = _dispatcher.Dispatch(new GetUsersQuery());

            var rs = auditLogs.Join(users, x => x.UserId, y => y.Id,
                (x, y) => new AuditLogEntryDTO
                {
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
}
