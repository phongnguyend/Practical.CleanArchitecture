using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkDelete;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkInsert;
using MapItEasy;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Persistence.Repositories;

public class SmsMessageRepository : Repository<SmsMessage, Guid>, ISmsMessageRepository
{
    private static readonly IMapper _mapper = new ExpressionMapper();
    private readonly AdsDbContext _dbContext;

    public SmsMessageRepository(AdsDbContext dbContext,
        IDateTimeProvider dateTimeProvider)
        : base(dbContext, dateTimeProvider)
    {
        _dbContext = dbContext;
    }

    public async Task<int> ArchiveMessagesAsync(CancellationToken cancellationToken = default)
    {
        var archivedDate = DateTime.Now.AddDays(-30);

        var messagesToArchive = _dbContext.Set<SmsMessage>()
        .Where(x => x.CreatedDateTime < archivedDate)
        .ToList();

        if (messagesToArchive.Count == 0)
        {
            return 0;
        }

        var archivedMessages = messagesToArchive.Select(x => _mapper.Map<SmsMessage, ArchivedSmsMessage>(x)).ToList();

        using (await UnitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken))
        {
            await _dbContext.BulkInsertAsync(archivedMessages,
                new BulkInsertOptions
                {
                    KeepIdentity = true
                }, cancellationToken: cancellationToken);
            await _dbContext.BulkDeleteAsync(messagesToArchive, cancellationToken: cancellationToken);
            await UnitOfWork.CommitTransactionAsync(cancellationToken);
        }

        return messagesToArchive.Count;
    }
}
