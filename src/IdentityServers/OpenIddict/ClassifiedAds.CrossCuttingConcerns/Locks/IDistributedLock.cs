using System;

namespace ClassifiedAds.CrossCuttingConcerns.Locks;

public interface IDistributedLock : IDisposable
{
    IDistributedLockScope Acquire(string lockName);

    IDistributedLockScope TryAcquire(string lockName);
}
