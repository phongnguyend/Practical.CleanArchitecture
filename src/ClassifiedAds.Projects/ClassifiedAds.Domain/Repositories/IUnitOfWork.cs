using System.Data;

namespace ClassifiedAds.Domain.Repositories
{
    public interface IUnitOfWork
    {
        int SaveChanges();

        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        void CommitTransaction();
    }
}
