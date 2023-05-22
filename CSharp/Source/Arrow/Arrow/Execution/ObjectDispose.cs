using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Execution
{
    /// <summary>
    /// Simplifies implementing the object disposed pattern
    /// </summary>
    public static class ObjectDispose
    {
        /// <summary>
        /// The initial value for a multi-threaded dispose solution
        /// </summary>
        public const int MultiThreadedNotDisposed = 0;

        /// <summary>
        /// The initial value for a single-threaded dispose solution
        /// </summary>
        public const bool SingleThreadedNotDisposed = false;

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
        /// Returns true if the flag indicates the caller is disposed, otherwise false
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool IsDisposed(ref bool flag)
        {
            return flag;
        }

        /// <summary>
        /// Throws an exception if the caller is already disposed
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="objectName"></param>
        /// <param name="reason"></param>
        /// <exception cref="ObjectDisposedException"></exception>
        public static void ThrowIfDisposed(ref int flag, string? objectName, [CallerMemberName] string? reason = null)
        {
            if(IsDisposed(ref flag)) throw new ObjectDisposedException(objectName, reason);
        }

        /// <summary>
        /// Throws an exception if the caller is already disposed
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="objectName"></param>
        /// <param name="reason"></param>
        /// <exception cref="ObjectDisposedException"></exception>
        public static void ThrowIfDisposed(ref bool flag, string? objectName, [CallerMemberName] string? reason = null)
        {
            if(IsDisposed(ref flag)) throw new ObjectDisposedException(objectName, reason);
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

        /// <summary>
        /// Attempts to transition the flag to disposed.
        /// If the flag already indicates the caller is disposed then returns false
        /// </summary>
        /// <param name="flag"></param>
        /// <returns>true if the caller should do their Dispose logic, false if they should not</returns>
        public static bool TryDispose(ref bool flag)
        {
            if(flag) return false;

            flag = true;
            return true;
        }
    }
}
