using System;

namespace ClassifiedAds.CrossCuttingConcerns.Locks;

public interface ILockManager
{
    bool AcquireLock(string entityName, string entityId, string ownerId, TimeSpan expirationIn);

    bool ExtendLock(string entityName, string entityId, string ownerId, TimeSpan expirationIn);

    bool ReleaseLock(string entityName, string entityId, string ownerId);

    bool ReleaseLocks(string ownerId);

    bool ReleaseExpiredLocks();

    void EnsureAcquiringLock(string entityName, string entityId, string ownerId, TimeSpan expirationIn);
}
