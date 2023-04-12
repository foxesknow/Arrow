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
                LockMode.BucketReadWrite    => new BucketReadWriteLockPolicy<TKey>(37),
                _                           => throw new Exception($"unexpected lock mode: {lockMode}")
            };
        }
    }
}