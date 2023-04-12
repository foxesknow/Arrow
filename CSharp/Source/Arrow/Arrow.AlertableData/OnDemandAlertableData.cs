using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Collections.Concurrent;

namespace Arrow.AlertableData
{
    /// <summary>
    /// An implementation of alertable data that allows for on-demand subscriptions
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public sealed class OnDemandAlertableData<TKey, TData> : AlertableDataBase<TKey, TData> where TData : class where TKey : notnull
    {
        private readonly IOptimizedConcurrentDictionary<TKey, StateData> m_Data;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="lockPolicy"></param>
        public OnDemandAlertableData(ILockPolicy<TKey> lockPolicy) : this(lockPolicy, null)
        {
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="lockPolicy"></param>
        /// <param name="comparer"></param>
        public OnDemandAlertableData(ILockPolicy<TKey> lockPolicy, IEqualityComparer<TKey>? comparer) : base(lockPolicy, comparer)
        {
            var numberOfBuckets = CalculateNumberOfBuckets();
            m_Data = BucketOptimizedConcurrentDictionary.Make<TKey, StateData>(numberOfBuckets, this.Comparer);
        }

        /// <summary>
        /// Sets up a subscription
        /// </summary>
        /// <param name="key"></param>
        /// <param name="initialState"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Subscribe(TKey key, TData? initialState)
        {
            if(key is null) throw new ArgumentNullException(nameof(key));

            // Do nothing if we're already subscribed
            if(ContainsKey(key)) return false;

            var lockPolicy = this.LockPolicy.Allocate(this.Comparer, key);
            var stateData = new StateData(lockPolicy, initialState);

            if(m_Data.TryAdd(key, stateData))
            {
                return true;
            }
            else
            {
                // The data was there when we called TryAdd. If may have been added on a different thread.
                // We need to tidy up by reclaiming any lock as it may have resources attached.
                this.LockPolicy.Reclaim(this.Comparer, key, lockPolicy);
            }

            return false;
        }

        protected override IEnumerable<StateData> AllStateData()
        {
            return m_Data.Values();
        }

        protected override bool ContainsKey(TKey key)
        {
            return m_Data.ContainsKey(key);
        }

        protected override bool TryGetStateData(TKey key, [NotNullWhen(true)] out StateData? stateData)
        {
            return m_Data.TryGetValue(key, out stateData);
        }
    }
}
