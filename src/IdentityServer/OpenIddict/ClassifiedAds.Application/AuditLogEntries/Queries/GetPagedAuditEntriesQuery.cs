using ClassifiedAds.Application.AuditLogEntries.DTOs;
using ClassifiedAds.Application.Common.DTOs;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.Domain.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Application.AuditLogEntries.Queries;

public class GetPagedAuditEntriesQuery : AuditLogEntryQueryOptions, IQuery<Paged<AuditLogEntryDTO>>
{
    public int Page { get; set; }

    public int PageSize { get; set; }
}

internal class GetPagedAuditEntriesQueryHandler : IQueryHandler<GetPagedAuditEntriesQuery, Paged<AuditLogEntryDTO>>
{
    private readonly IAuditLogEntryRepository _auditLogEntryRepository;
    private readonly IUserRepository _userRepository;

    public GetPagedAuditEntriesQueryHandler(IAuditLogEntryRepository auditLogEntryRepository, IUserRepository userRepository)
    {
        _auditLogEntryRepository = auditLogEntryRepository;
        _userRepository = userRepository;
    }

    public async Task<Paged<AuditLogEntryDTO>> HandleAsync(GetPagedAuditEntriesQuery queryOptions, CancellationToken cancellationToken = default)
    {
        var query = _auditLogEntryRepository.Get(queryOptions);
        var users = _userRepository.GetQueryableSet();

        var result = new Paged<AuditLogEntryDTO>
        {
            TotalItems = query.Count(),
        };

        var auditLogs = query.OrderByDescending(x => x.CreatedDateTime)
            .Paged(queryOptions.Page, queryOptions.PageSize);

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

        result.Items = await _userRepository.ToListAsync(rs);

        return result;
    }
}
