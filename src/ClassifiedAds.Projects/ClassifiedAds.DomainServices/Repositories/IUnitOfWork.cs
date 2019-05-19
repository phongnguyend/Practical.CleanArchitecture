using System.Data;

namespace ClassifiedAds.DomainServices.Repositories
{
    public interface IUnitOfWork
    {
        void SaveChanges();

        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        void CommitTransaction();
    }
}
