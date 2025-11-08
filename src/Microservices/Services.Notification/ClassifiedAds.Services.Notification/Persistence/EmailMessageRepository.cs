using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Services.Notification.Entities;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkDelete;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkInsert;
using MapItEasy;
using System.Data;

namespace ClassifiedAds.Services.Notification.Persistence;

public class EmailMessageRepository : Repository<EmailMessage, Guid>, IEmailMessageRepository
{
    private static readonly IMapper _mapper = new ExpressionMapper();
    private readonly NotificationDbContext _dbContext;

    public EmailMessageRepository(NotificationDbContext dbContext,
        IDateTimeProvider dateTimeProvider)
        : base(dbContext, dateTimeProvider)
    {
        _dbContext = dbContext;
    }

    public async Task<int> ArchiveMessagesAsync(CancellationToken cancellationToken = default)
    {
        var archivedDate = DateTime.Now.AddDays(-30);

        var messagesToArchive = _dbContext.Set<EmailMessage>()
        .Where(x => x.CreatedDateTime < archivedDate)
        .ToList();

        if (messagesToArchive.Count == 0)
        {
            return 0;
        }

        var archivedMessages = messagesToArchive.Select(x => _mapper.Map<EmailMessage, ArchivedEmailMessage>(x)).ToList();

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
