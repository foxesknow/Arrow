using System;

using Arrow.AlertableData;

namespace UnitTests.Arrow.AlertableData
{
    static class LockPolicyFactory
    {
        public static ILockPolicy<TKey> Make<TKey>(LockMode lockMode)
        {
            return lockMode switch
            {
                LockMode.Monitor            => new MonitorLockPolicy<TKey>(),
                LockMode.ReadWrite          => new ReadWriteLockPolicy<TKey>(),
                LockMode.BucketMonitor      => new BucketLockPolicy<TKey>(37, new MonitorLockPolicy<TKey>()),
                LockMode.BucketReadWrite    => new BucketLockPolicy<TKey>(37, new ReadWriteLockPolicy<TKey>()),
                LockMode.SpinLock           => new SpinLockPolicy<TKey>(),
                _                           => throw new Exception($"unexpected lock mode: {lockMode}")
            };
        }
    }
}