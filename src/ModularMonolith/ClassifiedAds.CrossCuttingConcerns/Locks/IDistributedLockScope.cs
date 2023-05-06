using System;

namespace ClassifiedAds.CrossCuttingConcerns.Locks;

public interface IDistributedLockScope : IDisposable
{
    bool StillHoldingLock();
}
