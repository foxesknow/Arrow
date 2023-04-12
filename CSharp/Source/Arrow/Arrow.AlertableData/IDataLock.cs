namespace Arrow.AlertableData
{
    /// <summary>
    /// Defines the locking requirements for alertable data.
    /// Locks are not required to be reentrant
    /// </summary>
    public interface IDataLock
    {
        /// <summary>
        /// Enters a lock for reading
        /// </summary>
        /// <param name="lockTaken"></param>
        void EnterRead(ref bool lockTaken);
        
        /// <summary>
        /// Exits a read lock
        /// </summary>
        /// <param name="lockTaken"></param>
        void ExitRead(bool lockTaken);

        /// <summary>
        /// Enters a lock for writing
        /// </summary>
        /// <param name="lockTaken"></param>
        void EnterWrite(ref bool lockTaken);

        /// <summary>
        /// Exits a write lock
        /// </summary>
        /// <param name="lockTaken"></param>
        void ExitWrite(bool lockTaken);
    }
}