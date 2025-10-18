using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Application.FileEntries.Queries;

public class GetFileEntriesQuery : IQuery<List<FileEntry>>
{
}

internal class GetFileEntriesQueryHandler : IQueryHandler<GetFileEntriesQuery, List<FileEntry>>
{
    private readonly IRepository<FileEntry, Guid> _repository;

    public GetFileEntriesQueryHandler(IRepository<FileEntry, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<List<FileEntry>> HandleAsync(GetFileEntriesQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.ToListAsync(_repository.GetQueryableSet().Where(x => !x.Deleted));
    }
}
