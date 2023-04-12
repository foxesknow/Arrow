using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.AlertableData
{
    /// <summary>
    /// An implementation of alertable data that subscribes up front.
    /// This means we can be lock free when doing lookups
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public sealed class UpFrontAlertableData<TKey, TData> : AlertableDataBase<TKey, TData> where TData : class where TKey : notnull
    {
        private readonly int m_NumberOfBucket;
        private readonly Wrapper[] m_Buckets;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="lockPolicy"></param>
        /// <param name="subscriptions"></param>
        public UpFrontAlertableData(ILockPolicy<TKey> lockPolicy, IEnumerable<(TKey Key, TData? InitialState)> subscriptions)
            :this(lockPolicy, null, subscriptions)
        {
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="lockPolicy"></param>
        /// <param name="comparer"></param>
        /// <param name="subscriptions"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArrowException"></exception>
        public UpFrontAlertableData(ILockPolicy<TKey> lockPolicy, IEqualityComparer<TKey>? comparer, IEnumerable<(TKey Key, TData? InitialState)> subscriptions)
            : base(lockPolicy, comparer)
        {
            if(subscriptions is null) throw new ArgumentNullException(nameof(subscriptions));

            m_NumberOfBucket = CalculateNumberOfBuckets();
            var dictionaries = Enumerable.Range(1, m_NumberOfBucket)
                                         .Select(_ => new Dictionary<TKey, StateData>(this.Comparer))
                                         .ToArray();

            foreach(var subscription in subscriptions)
            {
                var key = subscription.Key;
                if(key is null) throw new ArgumentException("null key in sequence", nameof(subscriptions));

                var bucket = SelectBucket(key);
                var dictionary = dictionaries[bucket];

                if(dictionary.ContainsKey(key)) throw new ArrowException($"already subscribed to {key}");

                var sync = this.LockPolicy.Allocate(this.Comparer, key);
                var stateData = new StateData(sync, subscription.InitialState);
                dictionary.Add(key, stateData);
            }

            m_Buckets = dictionaries.Select(d => new Wrapper(d)).ToArray();
        }

        protected override IEnumerable<StateData> AllStateData()
        {
            return m_Buckets.SelectMany(bucket => bucket.Value.Values);
        }

        protected override bool ContainsKey(TKey key)
        {
            return SelectDictionary(key).ContainsKey(key);
        }

        protected override bool TryGetStateData(TKey key, [NotNullWhen(true)] out StateData? stateData)
        {
            return SelectDictionary(key).TryGetValue(key, out stateData);
        }

        private int SelectBucket(TKey key)
        {
            var hash = this.Comparer.GetHashCode(key) & int.MaxValue;
            return hash % m_NumberOfBucket;
        }

        private IReadOnlyDictionary<TKey, StateData> SelectDictionary(TKey key)
        {
            var bucket = SelectBucket(key);
            return m_Buckets[bucket].Value;
        }

        /// <summary>
        /// The wrapper is to avoid covariance checks when access the array, which are done on reads.
        /// It's a micro-optimization, but why not!
        /// </summary>
        private readonly struct Wrapper
        {
            public Wrapper(IReadOnlyDictionary<TKey, StateData> value)
            {
                this.Value = value;
            }

            public readonly IReadOnlyDictionary<TKey, StateData> Value{get;}
        }
    }
}
