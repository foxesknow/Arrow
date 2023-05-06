using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Arrow.Calendar;
using Arrow.Execution;
using Microsoft.SqlServer.Server;

#nullable enable

namespace Arrow.Logging.Loggers
{
    /// <summary>
    /// A base class for writing custom loggers
    /// </summary>
    public abstract class BaseLog : ILog
    {
        private static readonly LoggingInfo DebugLevel = new(LogLevel.Debug, "[DEBUG] ", ConsoleColor.Gray);
        private static readonly LoggingInfo InfoLevel =  new(LogLevel.Info,  "[INFO ] ", ConsoleColor.White);
        private static readonly LoggingInfo WarnLevel =  new(LogLevel.Warn,  "[WARN ] ", ConsoleColor.DarkYellow);
        private static readonly LoggingInfo ErrorLevel = new(LogLevel.Error, "[ERROR] ", ConsoleColor.Magenta);
        private static readonly LoggingInfo FatalLevel = new(LogLevel.Fatal, "[FATAL] ", ConsoleColor.Red);

        private bool m_DebugEnabled = true;
        private bool m_InfoEnabled = true;
        private bool m_WarnEnabled = true;
        private bool m_ErrorEnabled = true;
        private bool m_FatalEnabled = true;

        private LogLevel m_LogLevel = LogLevel.All;

        protected BaseLog()
        {
        }

        /// <summary>
        /// How to log the date and time
        /// </summary>
        public DateTimeMode DateTimeMode{get; init;}

        /// <summary>
        /// What to log. The default is everything
        /// </summary>
        public LogLevel LogLevel
        {
            get{return m_LogLevel;}
            set{ApplyLogLevel(value);}
        }

        /// <summary>
        /// Specified if the log output should include the log level
        /// </summary>
        public bool AddLogLevel{get; init;} = false;

        private void ApplyLogLevel(LogLevel logLevel)
        {
            m_LogLevel = logLevel;

            m_DebugEnabled = (logLevel & LogLevel.Debug) != 0;
            m_InfoEnabled = (logLevel & LogLevel.Info) != 0;
            m_WarnEnabled = (logLevel & LogLevel.Warn) != 0;
            m_ErrorEnabled = (logLevel & LogLevel.Error) != 0;
            m_FatalEnabled = (logLevel & LogLevel.Fatal) != 0;
        }

        /// <summary>
        /// Writes a log line to wherever it needs to go
        /// </summary>
        /// <param name="consoleLevel"></param>
        /// <param name="line"></param>
        protected abstract void WriteLine(in LoggingInfo consoleLevel, object line);

        private static object FormatLine(DateTimeMode dateTimeMode, bool addLogLevel, in LoggingInfo loggingInfo, object message)
        {
            var time = MakeTime(dateTimeMode);

            return (addLogLevel, time) switch
            {
                (true, null)    => $"{loggingInfo.Prefix}{message}",
                (true, _)       => $"{time} {loggingInfo.Prefix}{message}",
                (false, null)   => message,
                (false, _)      => $"{time} {message}",
            };
        }

        private static string? MakeTime(DateTimeMode mode)
        {
            return mode switch
            {
                DateTimeMode.Time     => Clock.Now.TimeOfDay.ToString(@"hh\:mm\:ss\.fff"),
                DateTimeMode.DateTime => Clock.Now.ToString(@"yyyyMMdd-HH\:mm\:ss\.fff"),
                _                            => null
            };
        }

        private void Log(in LoggingInfo loggingInfo, object message)
        {
            var self = this;
            MethodCall.AllowFail((self, loggingInfo, message), static state =>
            {
                var line = FormatLine(state.self.DateTimeMode, state.self.AddLogLevel, state.loggingInfo, state.message);

                state.self.WriteLine(in state.loggingInfo, line);
            });
        }

        private void Log(in LoggingInfo loggingInfo, object message, Exception exception)
        {
            var self = this;
            MethodCall.AllowFail((self, loggingInfo, message, exception), static state =>
            {
                var line1 = FormatLine(state.self.DateTimeMode, state.self.AddLogLevel, state.loggingInfo, state.message);
                var line2 = FormatLine(state.self.DateTimeMode, state.self.AddLogLevel, state.loggingInfo, state.exception);
                
                var combinedLine = string.Concat(line1, Environment.NewLine, line2);
                state.self.WriteLine(in state.loggingInfo, combinedLine);
            });
        }

        private void Log(in LoggingInfo loggingInfo, string format, object? arg0)
        {
            var self = this;
            MethodCall.AllowFail((self, loggingInfo, format, arg0), static state =>
            {
                var message = string.Format(state.format, state.arg0);
                var line = FormatLine(state.self.DateTimeMode, state.self.AddLogLevel, state.loggingInfo, message);

                state.self.WriteLine(in state.loggingInfo, line);
            });
        }

        private void Log(in LoggingInfo loggingInfo, string format, object? arg0, object? arg1)
        {
            var self = this;
            MethodCall.AllowFail((self, loggingInfo, format, arg0, arg1), static state =>
            {
                var message = string.Format(state.format, state.arg0, state.arg1);
                var line = FormatLine(state.self.DateTimeMode, state.self.AddLogLevel, state.loggingInfo, message);

                state.self.WriteLine(in state.loggingInfo, line);
            });
        }

        private void Log(in LoggingInfo loggingInfo, string format, object? arg0, object? arg1, object? arg2)
        {
            var self = this;
            MethodCall.AllowFail((self, loggingInfo, format, arg0, arg1, arg2), static state =>
            {
                var message = string.Format(state.format, state.arg0, state.arg1, state.arg2);
                var line = FormatLine(state.self.DateTimeMode, state.self.AddLogLevel, state.loggingInfo, message);

                state.self.WriteLine(in state.loggingInfo, line);
            });
        }

        private void Log(in LoggingInfo loggingInfo, string format, params object?[] args)
        {
            var self = this;
            MethodCall.AllowFail((self, loggingInfo, format, args), static state =>
            {
                var message = string.Format(state.format, state.args);
                var line = FormatLine(state.self.DateTimeMode, state.self.AddLogLevel, state.loggingInfo, message);

                state.self.WriteLine(in state.loggingInfo, line);
            });
        }

        private void Log(in LoggingInfo loggingInfo, IFormatProvider provider, string format, params object?[] args)
        {
            var self = this;
            MethodCall.AllowFail((self, loggingInfo, provider, format, args), static state =>
            {
                var message = string.Format(state.provider, state.format, state.args);
                var line = FormatLine(state.self.DateTimeMode, state.self.AddLogLevel, state.loggingInfo, message);

                state.self.WriteLine(in state.loggingInfo, line);
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
