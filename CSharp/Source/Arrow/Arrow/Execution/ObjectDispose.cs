using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Execution
{
    /// <summary>
    /// Simplifies implementing the object disposed pattern in a multithreaded environment
    /// </summary>
    public static class ObjectDispose
    {
        public const int NotDisposed = 0;

        /// <summary>
        /// Returns true if the flag indicates the caller is disposed, otherwise false
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool IsDisposed(ref int flag)
        {
            return Interlocked.CompareExchange(ref flag, 0, 0) == 1;
        }

        /// <summary>
        /// Throws an exception if the caller is already disposed
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="objectName"></param>
        /// <exception cref="ObjectDisposedException"></exception>
        public static void ThrowIfDisposed(ref int flag, string? objectName)
        {
            if(IsDisposed(ref flag)) throw new ObjectDisposedException(objectName);
        }

        /// <summary>
        /// Throws an exception if the caller is already disposed
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="objectName"></param>
        /// <param name="message"></param>
        /// <exception cref="ObjectDisposedException"></exception>
        public static void ThrowIfDisposed(ref int flag, string? objectName, string? message)
        {
            if(IsDisposed(ref flag)) throw new ObjectDisposedException(objectName, message);
        }

        /// <summary>
        /// Attempts to transition the flag to disposed.
        /// If the flag already indicates the caller is disposed then returns false
        /// </summary>
        /// <param name="flag"></param>
        /// <returns>true if the caller should do their Dispose logic, false if they should not</returns>
        public static bool TryDispose(ref int flag)
        {
            return Interlocked.CompareExchange(ref flag, 1, 0) == 0;
        }
    }
}
