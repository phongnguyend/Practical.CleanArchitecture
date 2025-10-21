using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Services.Notification.Entities;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkDelete;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkInsert;
using MapItEasy;
using System.Data;

namespace ClassifiedAds.Services.Notification.Persistence;

public class SmsMessageRepository : Repository<SmsMessage, Guid>, ISmsMessageRepository
{
    private static readonly IMapper _mapper = new ExpressionMapper();
    private readonly NotificationDbContext _dbContext;

    public SmsMessageRepository(NotificationDbContext dbContext,
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
            await _dbContext.BulkInsertAsync(archivedMessages, opt => opt.KeepIdentity = true, cancellationToken: cancellationToken);
            await _dbContext.BulkDeleteAsync(messagesToArchive, cancellationToken: cancellationToken);
            await UnitOfWork.CommitTransactionAsync(cancellationToken);
        }

        return messagesToArchive.Count;
    }
}
