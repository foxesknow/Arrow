using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Numeric;

namespace Arrow.AlertableData
{
    public abstract class AlertableDataBase<TKey, TData> :
        IAlertableDataConsumer<TKey, TData>,
        IAlertableDataProducer<TKey, TData>,
        IDisposable
        where TData : class where TKey : notnull
    {        
        protected AlertableDataBase(ILockPolicy<TKey> lockPolicy, IEqualityComparer<TKey>? comparer)
        {
            if(lockPolicy is null) throw new ArgumentNullException(nameof(lockPolicy));

            this.Comparer = (comparer ?? EqualityComparer<TKey>.Default);
            this.LockPolicy = lockPolicy;
        }

        public void Dispose()
        {
            foreach(var stateData in AllStateData())
            {
                stateData.Dispose();
            }
        }

        /// <summary>
        /// The lock policy being used
        /// </summary>
        protected ILockPolicy<TKey> LockPolicy{get;}

        /// <summary>
        /// The comparer being used
        /// </summary>
        protected IEqualityComparer<TKey> Comparer{get;}
        
        /// <inheritdoc/>
        public bool IsSubscribed(TKey key)
        {
            return ContainsKey(key);
        }

        /// <inheritdoc/>
        public bool Publish<TState>(TKey key, TState state, DataWriter<TKey, TState, TData> writer)
        {
            if(TryGetStateData(key, out var stateData) == false) return false;

            var lockTaken = false;

            try
            {
                stateData.Sync.EnterWrite(ref lockTaken);

                var currentState = stateData.Data;
                var newState = writer(key, state, currentState);

                // Only update .Data if the reference has changed to aboid invalidating the cpu cachse
                if(object.ReferenceEquals(currentState, newState) == false)
                {
                    stateData.Data = newState;
                }
            }
            finally
            {
                stateData.Sync.ExitWrite(lockTaken);
            }

            return true;
        }

        /// <inheritdoc/>
        public ReadLock Read(TKey key, out ReadResult<TData> readResult)
        {
            if(TryGetStateData(key, out var stateData) == false) 
            {
                readResult = default;
                return new();
            }

            var dataLock = stateData.Sync;
            var lockTaken = false;

            try
            {
                // If the locks aren't recursive then this will fail
                dataLock.EnterRead(ref lockTaken);
                readResult = new(true, stateData.Data);
                return new(lockTaken, dataLock);
            }
            catch
            {
                readResult = default;
                dataLock.ExitRead(lockTaken);
                throw;
            }
        }

        /// <inheritdoc/>
        public (bool Succeeded, TResult Data) TryRead<TState, TResult>(TKey key, TState state, DataReader<TKey, TState, TData, TResult> reader)
        {
            if(TryGetStateData(key, out var stateData) == false) 
            {
                return (false, default!);
            }

            var lockTaken = false;

            try
            {
                stateData.Sync.EnterRead(ref lockTaken);

                var data = stateData.Data;
                if(data is null) return (false, default!);

                return new (true, reader(key, state, data));
            }
            finally
            {
                stateData.Sync.ExitRead(lockTaken);
            }
        }

        /// <inheritdoc/>
        public bool TryReadByRef<TState, TResult>(TKey key, TState state, ref TResult result, DataReaderByRef<TKey, TState, TData, TResult> reader)
        {
            if(TryGetStateData(key, out var stateData) == false) 
            {
                return false;
            }

            var lockTaken = false;

            try
            {
                stateData.Sync.EnterRead(ref lockTaken);

                var data = stateData.Data;
                if(data is null) return false;

                reader(key, state, data, ref result);
                return true;
            }
            finally
            {
                stateData.Sync.ExitRead(lockTaken);
            }
        }

        /// <summary>
        /// Returns all state data managed by a derived class
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<StateData> AllStateData();

        /// <summary>
        /// Attempts to get a piece of state data
        /// </summary>
        /// <param name="key"></param>
        /// <param name="stateData"></param>
        /// <returns></returns>
        protected abstract bool TryGetStateData(TKey key, [NotNullWhen(true)] out StateData? stateData);

        /// <summary>
        /// Checks to see if a key exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected abstract bool ContainsKey(TKey key);

        protected static int CalculateNumberOfBuckets()
        {
            // Most alertable data is IO bound, so we'll scale out
            var scaledCores = Environment.ProcessorCount * 2;

            if(PrimeSequence.First1000.TryGetFirstPrimeAfter(scaledCores, out var buckets))
            {
                return buckets;
            }

            // If we get here then that means we've got more than 7919 (wow!)
            return scaledCores;
        }

        protected sealed class StateData : IDisposable
        {
            public StateData(IDataLock sync, TData? data)
            {
                this.Sync = sync;
                this.Data = data;
            }

            public readonly IDataLock Sync;

            public TData? Data;

            public void Dispose()
            {
                if(this.Sync is IDisposable d)
                {
                    d.Dispose();
                }
            }
        }
    }
}
