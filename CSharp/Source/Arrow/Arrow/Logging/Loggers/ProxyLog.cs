using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arrow.Execution;
using Microsoft.SqlServer.Server;

#nullable enable

namespace Arrow.Logging.Loggers
{
    /// <summary>
    /// A log that sits between the user and the actual log.
    /// The log can be changed on the fly
    /// </summary>
    public sealed class ProxyLog : ILog, IPropertyContext, IForContext
    {
        private static ILog s_Log = NullLog.Instance;        

        /// <summary>
        /// Registers a new log
        /// </summary>
        /// <param name="log"></param>
        /// <returns>The previous proxied log</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ILog Register(ILog log)
        {
            if(log is null) throw new ArgumentNullException(nameof(log));

            return Interlocked.Exchange(ref s_Log, log);
        }

        /// <summary>
        /// Switches the proxy to a new log
        /// </summary>
        /// <param name="log"></param>
        /// <returns>A disposable item that will swithc the proxy back to the original log</returns>
        public static IDisposable Switch(ILog log)
        {
            var previousLog = Register(log);
            return new Disposer(() => Register(previousLog));
        }

        ILog IForContext.ForContext<T>()
        {
            if(s_Log is IForContext c)
            {
                return c.ForContext<T>();
            }

            return this;
        }

        ILog IForContext.ForContext(Type? type)
        {
            if(s_Log is IForContext c)
            {
                return c.ForContext(type);
            }

            return this;
        }

        ILog IForContext.ForContext(string? name)
        {
            if(s_Log is IForContext c)
            {
                return c.ForContext(name);
            }

            return this;
        }

        IPropertyPusher? IPropertyContext.GetPusher()
        {
            if(s_Log is IPropertyContext pusher)
            {
                return pusher.GetPusher();
            }

            return null;
        }

        bool ILog.IsDebugEnabled => s_Log.IsDebugEnabled;

        bool ILog.IsInfoEnabled => s_Log.IsInfoEnabled;

        bool ILog.IsWarnEnabled => s_Log.IsWarnEnabled;

        bool ILog.IsErrorEnabled => s_Log.IsErrorEnabled;

        bool ILog.IsFatalEnabled => s_Log.IsFatalEnabled;

        void ILog.Debug(object message)
        {
            s_Log.Debug(message);
        }

        void ILog.Debug(object message, Exception exception)
        {
            s_Log.Debug(message, exception);
        }

        void ILog.DebugFormat(string format, params object?[] args)
        {
            s_Log.DebugFormat(format, args);
        }

        void ILog.DebugFormat(string format, object? arg0)
        {
            s_Log.DebugFormat(format, arg0);
        }

        void ILog.DebugFormat(string format, object? arg0, object? arg1)
        {
            s_Log.DebugFormat(format, arg0, arg1);
        }

        void ILog.DebugFormat(string format, object? arg0, object? arg1, object? arg2)
        {
            s_Log.DebugFormat(format, arg0, arg1, arg2);
        }

        void ILog.DebugFormat(IFormatProvider provider, string format, params object?[] args)
        {
            s_Log.DebugFormat(provider, format, args);
        }

        void ILog.Error(object message)
        {
            s_Log.Error(message);
        }

        void ILog.Error(object message, Exception exception)
        {
            s_Log.Error(message, exception);
        }

        void ILog.ErrorFormat(string format, params object?[] args)
        {
            s_Log.ErrorFormat(format, args);
        }

        void ILog.ErrorFormat(string format, object? arg0)
        {
            s_Log.ErrorFormat(format, arg0);
        }

        void ILog.ErrorFormat(string format, object? arg0, object? arg1)
        {
            s_Log.ErrorFormat(format, arg0, arg1);
        }

        void ILog.ErrorFormat(string format, object? arg0, object? arg1, object? arg2)
        {
            s_Log.ErrorFormat(format, arg0, arg1, arg2);
        }

        void ILog.ErrorFormat(IFormatProvider provider, string format, params object?[] args)
        {
            s_Log.ErrorFormat(provider, format, args);
        }

        void ILog.Fatal(object message)
        {
            s_Log.Fatal(message);
        }

        void ILog.Fatal(object message, Exception exception)
        {
            s_Log.Fatal(message, exception);
        }

        void ILog.FatalFormat(string format, params object?[] args)
        {
            s_Log.FatalFormat(format, args);
        }

        void ILog.FatalFormat(string format, object? arg0)
        {
            s_Log.FatalFormat(format, arg0);
        }

        void ILog.FatalFormat(string format, object? arg0, object? arg1)
        {
            s_Log.FatalFormat(format, arg0, arg1);
        }

        void ILog.FatalFormat(string format, object? arg0, object? arg1, object? arg2)
        {
            s_Log.FatalFormat(format, arg0, arg1, arg2);
        }

        void ILog.FatalFormat(IFormatProvider provider, string format, params object?[] args)
        {
            s_Log.FatalFormat(provider, format, args);
        }

        void ILog.Info(object message)
        {
            s_Log.Info(message);
        }

        void ILog.Info(object message, Exception exception)
        {
            s_Log.Info(message, exception);
        }

        void ILog.InfoFormat(string format, params object[] args)
        {
            s_Log.InfoFormat(format, args);
        }

        void ILog.InfoFormat(string format, object? arg0)
        {
            s_Log.InfoFormat(format, arg0);
        }

        void ILog.InfoFormat(string format, object? arg0, object? arg1)
        {
            s_Log.InfoFormat(format, arg0, arg1);
        }

        void ILog.InfoFormat(string format, object? arg0, object? arg1, object? arg2)
        {
            s_Log.InfoFormat(format, arg0, arg1, arg2);
        }

        void ILog.InfoFormat(IFormatProvider provider, string format, params object?[] args)
        {
            s_Log.InfoFormat(provider, format, args);
        }

        void ILog.Warn(object message)
        {
            s_Log.Warn(message);
        }

        void ILog.Warn(object message, Exception exception)
        {
            s_Log.Warn(message, exception);
        }

        void ILog.WarnFormat(string format, params object?[] args)
        {
            s_Log.WarnFormat(format, args);
        }

        void ILog.WarnFormat(string format, object? arg0)
        {
            s_Log.WarnFormat(format, arg0);
        }

        void ILog.WarnFormat(string format, object? arg0, object? arg1)
        {
            s_Log.WarnFormat(format, arg0, arg1);
        }

        void ILog.WarnFormat(string format, object? arg0, object? arg1, object? arg2)
        {
            s_Log.WarnFormat(format, arg0, arg1, arg2);
        }

        void ILog.WarnFormat(IFormatProvider provider, string format, params object?[] args)
        {
            s_Log.WarnFormat(provider, format, args);
        }
    }
}
