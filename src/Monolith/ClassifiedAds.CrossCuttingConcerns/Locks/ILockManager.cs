using System;

namespace ClassifiedAds.CrossCuttingConcerns.Locks
{
    public interface ILockManager
    {
        bool AcquireLock(string entityName, string entityId, string ownerId, TimeSpan expirationIn);

        bool ReleaseLock(string entityName, string entityId, string ownerId);

        bool ExtendLock(string entityName, string entityId, string ownerId, TimeSpan expirationIn);
    }
}
