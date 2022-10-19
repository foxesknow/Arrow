using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Calendar;
using Arrow.Execution;
using Microsoft.SqlServer.Server;

#nullable enable

namespace Arrow.Logging.Loggers
{
    public abstract class BaseConsoleLog : ILog
    {
        private static readonly ConsoleLevel DebugLevel = new("[DEBUG] ", ConsoleColor.Gray);
        private static readonly ConsoleLevel InfoLevel =  new("[INFO ] ", ConsoleColor.White);
        private static readonly ConsoleLevel WarnLevel =  new("[WARN ] ", ConsoleColor.DarkYellow);
        private static readonly ConsoleLevel ErrorLevel = new("[ERROR] ", ConsoleColor.Magenta);
        private static readonly ConsoleLevel FatalLevel = new("[FATAL] ", ConsoleColor.Red);

        private bool m_DebugEnabled = true;
        private bool m_InfoEnabled = true;
        private bool m_WarnEnabled = true;
        private bool m_ErrorEnabled = true;
        private bool m_FatalEnabled = true;

        private readonly TextWriter m_Out;
        private readonly bool m_Redirected;
        private readonly object m_SyncRoot;

        protected BaseConsoleLog(TextWriter output, bool redirected, object syncRoot)
        {
            m_Out = output;
            m_Redirected = redirected;
            m_SyncRoot = syncRoot;
        }

        private static object FormatLine(bool redirected, in ConsoleLevel level, object message)
        {
            if(redirected)
            {
                var time = MakeTime();
                return $"{time} {level.Prefix}{message}";
            }
            else
            {
                return message;
            }
        }

        private static string MakeTime()
        {
            var now = Clock.Now;
            return now.TimeOfDay.ToString(@"hh\:mm:\ss\.fff");
        }

        private void Log(in ConsoleLevel level, object message)
        {
            MethodCall.AllowFail((m_Redirected, m_SyncRoot, m_Out, level, message), static state =>
            {
                var line = FormatLine(state.m_Redirected, state.level, state.message);

                lock(state.m_SyncRoot)
                {
                    using(new ColorChange(state.level.Color, state.m_Redirected))
                    {
                        state.m_Out.WriteLine(line);
                    }
                }
            });
        }

        private void Log(in ConsoleLevel level, object message, Exception exception)
        {
            MethodCall.AllowFail((m_Redirected, m_SyncRoot, m_Out, level, message, exception), static state =>
            {
                var line1 = FormatLine(state.m_Redirected, state.level, state.message);
                var line2 = FormatLine(state.m_Redirected, state.level, state.exception);

                lock(state.m_SyncRoot)
                {
                    using(new ColorChange(state.level.Color, state.m_Redirected))
                    {
                        state.m_Out.WriteLine(line1);
                        state.m_Out.WriteLine(line2);
                    }
                }
            });
        }

        private void Log(in ConsoleLevel level, string format, params object?[] args)
        {
            MethodCall.AllowFail((m_Redirected, m_SyncRoot, m_Out, level, format, args), static state =>
            {
                var message = string.Format(state.format, state.args);
                var line = FormatLine(state.m_Redirected, state.level, message);

                lock(state.m_SyncRoot)
                {
                    using(new ColorChange(state.level.Color, state.m_Redirected))
                    {
                        state.m_Out.WriteLine(line);
                    }
                }
            });
        }

        private void Log(in ConsoleLevel level, IFormatProvider provider, string format, params object?[] args)
        {
            MethodCall.AllowFail((m_Redirected, m_SyncRoot, m_Out, level, provider, format, args), static state =>
            {
                var message = string.Format(state.provider, state.format, state.args);
                var line = FormatLine(state.m_Redirected, state.level, message);

                lock(state.m_SyncRoot)
                {
                    using(new ColorChange(state.level.Color, state.m_Redirected))
                    {
                        state.m_Out.WriteLine(line);
                    }
                }
            });
        }

        bool ILog.IsDebugEnabled
        {
            get{return m_DebugEnabled;}
        }

        bool ILog.IsInfoEnabled
        {
            get{return m_InfoEnabled;}
        }

        bool ILog.IsWarnEnabled
        {
            get{return m_WarnEnabled;}
        }

        bool ILog.IsErrorEnabled
        {
            get{return m_ErrorEnabled;}
        }

        bool ILog.IsFatalEnabled
        {
            get{return m_FatalEnabled;}
        }

        void ILog.Debug(object message)
        {
            if(m_DebugEnabled) Log(in DebugLevel, message);
        }

        void ILog.Debug(object message, Exception exception)
        {
            if(m_DebugEnabled) Log(in DebugLevel, exception);
        }

        void ILog.DebugFormat(string format, params object?[] args)
        {
            if(m_DebugEnabled) Log(in DebugLevel, format, args);
        }

        void ILog.DebugFormat(string format, object? arg0)
        {
            if(m_DebugEnabled) Log(in DebugLevel, format, arg0);
        }

        void ILog.DebugFormat(string format, object? arg0, object? arg1)
        {
            if(m_DebugEnabled) Log(in DebugLevel, format, arg0, arg1);
        }

        void ILog.DebugFormat(string format, object? arg0, object? arg1, object? arg2)
        {
            if(m_DebugEnabled) Log(in DebugLevel, format, arg0, arg1, arg2);
        }

        void ILog.DebugFormat(IFormatProvider provider, string format, params object?[] args)
        {
            if(m_DebugEnabled) Log(in DebugLevel, provider, format, args);
        }

        void ILog.Error(object message)
        {
            if(m_ErrorEnabled) Log(in ErrorLevel, message);
        }

        void ILog.Error(object message, Exception exception)
        {
            if(m_ErrorEnabled) Log(in ErrorLevel, message, exception);
        }

        void ILog.ErrorFormat(string format, params object?[] args)
        {
            if(m_ErrorEnabled) Log(in ErrorLevel, format, args);
        }

        void ILog.ErrorFormat(string format, object? arg0)
        {
            if(m_ErrorEnabled) Log(in ErrorLevel, format, arg0);
        }

        void ILog.ErrorFormat(string format, object? arg0, object? arg1)
        {
            if(m_ErrorEnabled) Log(in ErrorLevel, format, arg0, arg1);
        }

        void ILog.ErrorFormat(string format, object? arg0, object? arg1, object? arg2)
        {
            if(m_ErrorEnabled) Log(in ErrorLevel, format, arg0, arg1, arg2);
        }

        void ILog.ErrorFormat(IFormatProvider provider, string format, params object?[] args)
        {
            if(m_ErrorEnabled) Log(in ErrorLevel, provider, format, args);
        }

        void ILog.Fatal(object message)
        {
            if(m_FatalEnabled) Log(in FatalLevel, message);
        }

        void ILog.Fatal(object message, Exception exception)
        {
            if(m_FatalEnabled) Log(in FatalLevel, message, exception);
        }

        void ILog.FatalFormat(string format, params object?[] args)
        {
            if(m_FatalEnabled) Log(in FatalLevel, format, args);
        }

        void ILog.FatalFormat(string format, object? arg0)
        {
            if(m_FatalEnabled) Log(in FatalLevel, format, arg0);
        }

        void ILog.FatalFormat(string format, object? arg0, object? arg1)
        {
            if(m_FatalEnabled) Log(in FatalLevel, format, arg0, arg1);
        }

        void ILog.FatalFormat(string format, object? arg0, object? arg1, object? arg2)
        {
            if(m_FatalEnabled) Log(in FatalLevel, format, arg0, arg1, arg2);
        }

        void ILog.FatalFormat(IFormatProvider provider, string format, params object?[] args)
        {
            if(m_FatalEnabled) Log(in FatalLevel, provider, format, args);
        }

        void ILog.Info(object message)
        {
            if(m_InfoEnabled) Log(in InfoLevel, message);
        }

        void ILog.Info(object message, Exception exception)
        {
            if(m_InfoEnabled) Log(in InfoLevel, message, exception);
        }

        void ILog.InfoFormat(string format, params object?[] args)
        {
            if(m_InfoEnabled) Log(in InfoLevel, format, args);
        }

        void ILog.InfoFormat(string format, object? arg0)
        {
            if(m_InfoEnabled) Log(in InfoLevel, format, arg0);
        }

        void ILog.InfoFormat(string format, object? arg0, object? arg1)
        {
            if(m_InfoEnabled) Log(in InfoLevel, format, arg0, arg1);
        }

        void ILog.InfoFormat(string format, object? arg0, object? arg1, object? arg2)
        {
            if(m_InfoEnabled) Log(in InfoLevel, format, arg0, arg1, arg2);
        }

        void ILog.InfoFormat(IFormatProvider provider, string format, params object?[] args)
        {
            if(m_InfoEnabled) Log(in InfoLevel, provider, format, args);
        }

        void ILog.Warn(object message)
        {
            if(m_WarnEnabled) Log(in WarnLevel, message);
        }

        void ILog.Warn(object message, Exception exception)
        {
            if(m_WarnEnabled) Log(in WarnLevel, message, exception);
        }

        void ILog.WarnFormat(string format, params object?[] args)
        {
            if(m_WarnEnabled) Log(in WarnLevel, format, args);
        }

        void ILog.WarnFormat(string format, object? arg0)
        {
            if(m_WarnEnabled) Log(in WarnLevel, format, arg0);
        }

        void ILog.WarnFormat(string format, object? arg0, object? arg1)
        {
            if(m_WarnEnabled) Log(in WarnLevel, format, arg0, arg1);
        }

        void ILog.WarnFormat(string format, object? arg0, object? arg1, object? arg2)
        {
            if(m_WarnEnabled) Log(in WarnLevel, format, arg0, arg1, arg2);
        }

        void ILog.WarnFormat(IFormatProvider provider, string format, params object?[] args)
        {
            if(m_WarnEnabled) Log(in WarnLevel, provider, format, args);
        }
    }
}
