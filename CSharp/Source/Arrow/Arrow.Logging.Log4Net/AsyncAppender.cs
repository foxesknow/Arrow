using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Arrow.Logging.Async;

using log4net;
using log4net.Appender;
using log4net.Core;

namespace Arrow.Logging.Log4Net
{
	/// <summary>
	/// A log4net appender that defers logging to the AsyncLogWriter thread
	/// 
	/// To use just wrap your existing appender within this element:
	/// <![CDATA[ <appender name="FileAppender" type="Arrow.Logging.Log4Net.AsyncAppender, Arrow.Logging.Log4Net"> ]]>
	/// </summary>
	public class AsyncAppender : AppenderSkeleton
	{
        private IAppender m_Appender;

        /// <summary>
        /// The appender that does the actual logging
        /// </summary>
        public IAppender Appender
        {
            get{return m_Appender;}
            set{m_Appender = value;}
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            IAppender appender = m_Appender;
            if(appender != null)
            {
                // As we're doing the logging on a different thread
                // we need to capture the thread information here
                loggingEvent.Fix = FixFlags.ThreadName | FixFlags.Exception;
                AsyncPropertyPusher.Combine(loggingEvent.Properties);

                LogData data = new LogData(appender, loggingEvent);
                AsyncLogWriter.Enqueue(data);
            }
        }

        /// <summary>
        /// Close the appender we delegate to
        /// </summary>
        protected override void OnClose()
        {
            IAppender appender = m_Appender;
            if(appender != null) appender.Close();

            base.OnClose();
        }

        class LogData : ILogData
        {
            public readonly IAppender Appender;
            public readonly LoggingEvent LoggingEvent;

            public LogData(IAppender appender, LoggingEvent loggingEvent)
            {
                Appender = appender;
                LoggingEvent = loggingEvent;
            }

            public void WriteToLog()
            {
                Appender.DoAppend(LoggingEvent);
            }
        }
    }
}
