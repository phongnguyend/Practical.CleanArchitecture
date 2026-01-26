using ClassifiedAds.Application.AuditLogEntries.DTOs;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Application.AuditLogEntries.Queries;

public class GetAuditEntriesQuery : AuditLogEntryQueryOptions, IQuery<List<AuditLogEntryDTO>>
{
}

internal class GetAuditEntriesQueryHandler : IQueryHandler<GetAuditEntriesQuery, List<AuditLogEntryDTO>>
{
    private readonly IAuditLogEntryReadOnlyRepository _auditLogEntryRepository;
    private readonly IReadOnlyRepository<User, Guid> _userRepository;

    public GetAuditEntriesQueryHandler(IAuditLogEntryReadOnlyRepository auditLogEntryRepository, IReadOnlyRepository<User, Guid> userRepository)
    {
        _auditLogEntryRepository = auditLogEntryRepository;
        _userRepository = userRepository;
    }

    public async Task<List<AuditLogEntryDTO>> HandleAsync(GetAuditEntriesQuery query, CancellationToken cancellationToken = default)
    {
        var auditLogs = _auditLogEntryRepository.Get(query);
        var users = _userRepository.GetQueryableSet();

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

        return await _userRepository.ToListAsync(rs.OrderByDescending(x => x.CreatedDateTime));
    }
}
