using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Arrow.Logging
{
    /// <summary>
    /// Places a prefix in front on each log line.
    /// This is useful for scoping log output to a particular domain
    /// </summary>
    public sealed class PrefixLog : ILog, IPropertyContext, IForContext
    {
        private readonly ILog m_Outer;

        private readonly Func<string>? m_PrefixGenerator;
        private readonly string? m_Prefix;

        public PrefixLog(ILog outer, string prefix)
        {
            if(outer is null) throw new ArgumentNullException(nameof(outer));
            if(prefix is null) throw new ArgumentNullException(nameof(prefix));

            m_Outer = outer;
            m_Prefix = prefix;
        }

        public PrefixLog(ILog outer, Func<string> prefixGenerator)
        {
            if(outer is null) throw new ArgumentNullException(nameof(outer));
            if(prefixGenerator is null) throw new ArgumentNullException(nameof(prefixGenerator));

            m_Outer = outer;
            m_PrefixGenerator = prefixGenerator;
        }

        ILog IForContext.ForContext<T>()
        {
            IForContext self = this;
            return self.ForContext(typeof(T));
        }

        ILog IForContext.ForContext(Type? type)
        {
            if(m_Outer is IForContext c)
            {
                var log = c.ForContext(type);

                if(m_Prefix is not null)
                {
                    return new PrefixLog(log, m_Prefix);
                }
                else
                {
                    return new PrefixLog(log, m_PrefixGenerator!);
                }
            }
            else
            {
                return this;
            }
        }

        ILog IForContext.ForContext(string? name)
        {
            if(m_Outer is IForContext c)
            {
                return c.ForContext(name);
            }

            return this;
        }

        IPropertyPusher? IPropertyContext.GetPusher()
        {
            if(m_Outer is IPropertyContext context)
            {
                return context.GetPusher();
            }

            return null;
        }

        bool ILog.IsDebugEnabled => m_Outer.IsDebugEnabled;

        bool ILog.IsInfoEnabled => m_Outer.IsInfoEnabled;

        bool ILog.IsWarnEnabled => m_Outer.IsWarnEnabled;

        bool ILog.IsErrorEnabled => m_Outer.IsErrorEnabled;

        bool ILog.IsFatalEnabled => m_Outer.IsFatalEnabled;

        void ILog.Debug(object message)
        {
            m_Outer.Debug(Combine(message));
        }

        void ILog.Debug(object message, Exception exception)
        {
            m_Outer.Debug(Combine(message), exception);
        }

        void ILog.DebugFormat(string format, params object?[] args)
        {
            m_Outer.DebugFormat(Combine(format), args);
        }

        void ILog.DebugFormat(string format, object? arg0)
        {
            m_Outer.DebugFormat(Combine(format), arg0);
        }

        void ILog.DebugFormat(string format, object? arg0, object? arg1)
        {
            m_Outer.DebugFormat(Combine(format), arg0, arg1);
        }

        void ILog.DebugFormat(string format, object? arg0, object? arg1, object? arg2)
        {
            m_Outer.DebugFormat(Combine(format), arg0, arg1, arg2);
        }

        void ILog.DebugFormat(IFormatProvider provider, string format, params object?[] args)
        {
            m_Outer.DebugFormat(provider, Combine(format), args);
        }

        void ILog.Error(object message)
        {
            m_Outer.Error(Combine(message));
        }

        void ILog.Error(object message, Exception exception)
        {
            m_Outer.Error(Combine(message), exception);
        }

        void ILog.ErrorFormat(string format, params object?[] args)
        {
            m_Outer.ErrorFormat(Combine(format), args);
        }

        void ILog.ErrorFormat(string format, object? arg0)
        {
            m_Outer.ErrorFormat(Combine(format), arg0);
        }

        void ILog.ErrorFormat(string format, object? arg0, object? arg1)
        {
            m_Outer.ErrorFormat(Combine(format), arg0, arg1);
        }

        void ILog.ErrorFormat(string format, object? arg0, object? arg1, object? arg2)
        {
            m_Outer.ErrorFormat(Combine(format), arg0, arg1, arg2);
        }

        void ILog.ErrorFormat(IFormatProvider provider, string format, params object?[] args)
        {
            m_Outer.ErrorFormat(provider, Combine(format), args);
        }

        void ILog.Fatal(object message)
        {
            m_Outer.Fatal(Combine(message));
        }

        void ILog.Fatal(object message, Exception exception)
        {
            m_Outer.Fatal(Combine(message));
        }

        void ILog.FatalFormat(string format, params object?[] args)
        {
            m_Outer.FatalFormat(Combine(format), args);
        }

        void ILog.FatalFormat(string format, object? arg0)
        {
            m_Outer.FatalFormat(Combine(format), arg0);
        }

        void ILog.FatalFormat(string format, object? arg0, object? arg1)
        {
            m_Outer.FatalFormat(Combine(format), arg0, arg1);
        }

        void ILog.FatalFormat(string format, object? arg0, object? arg1, object? arg2)
        {
            m_Outer.FatalFormat(Combine(format), arg0, arg1, arg2);
        }

        void ILog.FatalFormat(IFormatProvider provider, string format, params object?[] args)
        {
            m_Outer.FatalFormat(provider, Combine(format), args);
        }        

        void ILog.Info(object message)
        {
            m_Outer.Info(Combine(message));
        }

        void ILog.Info(object message, Exception exception)
        {
            m_Outer.Info(Combine(message), exception);
        }

        void ILog.InfoFormat(string format, params object[] args)
        {
            m_Outer.InfoFormat(Combine(format), args);
        }

        void ILog.InfoFormat(string format, object? arg0)
        {
            m_Outer.InfoFormat(Combine(format), arg0);
        }

        void ILog.InfoFormat(string format, object? arg0, object? arg1)
        {
            m_Outer.InfoFormat(Combine(format), arg0, arg1);
        }

        void ILog.InfoFormat(string format, object? arg0, object? arg1, object? arg2)
        {
            m_Outer.InfoFormat(Combine(format), arg0, arg1, arg2);
        }

        void ILog.InfoFormat(IFormatProvider provider, string format, params object?[] args)
        {
            m_Outer.InfoFormat(provider, Combine(format), args);
        }

        void ILog.Warn(object message)
        {
            m_Outer.Warn(Combine(message));
        }

        void ILog.Warn(object message, Exception exception)
        {
            m_Outer.Warn(Combine(message), exception);
        }

        void ILog.WarnFormat(string format, params object?[] args)
        {
            m_Outer.WarnFormat(Combine(format), args);
        }

        void ILog.WarnFormat(string format, object? arg0)
        {
            m_Outer.WarnFormat(Combine(format), arg0);
        }

        void ILog.WarnFormat(string format, object? arg0, object? arg1)
        {
            m_Outer.WarnFormat(Combine(format), arg0, arg1);
        }

        void ILog.WarnFormat(string format, object? arg0, object? arg1, object? arg2)
        {
            m_Outer.WarnFormat(Combine(format), arg0, arg1, arg2);
        }

        void ILog.WarnFormat(IFormatProvider provider, string format, params object?[] args)
        {
            m_Outer.WarnFormat(provider, Combine(format), args);
        }

        private object Combine(object message)
        {
            return Combine(message?.ToString());
        }

        private string Combine(string? message)
        {
            var prefix = m_Prefix ?? m_PrefixGenerator!();
            return string.Concat(prefix, " ", (message ?? ""));
        }
    }
}
