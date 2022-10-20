using System;
using System.Collections.Generic;
using System.Text;

#nullable disable

namespace Arrow.Logging.Loggers
{
	/// <summary>
	/// Log implementation that does nothing
	/// </summary>
	public class NullLog : ILog
	{
		/// <summary>
		/// A shareable instance of the class
		/// </summary>
		public static readonly ILog Instance=new NullLog();
	
		bool ILog.IsDebugEnabled
		{
			get{return false;}
		}

		bool ILog.IsInfoEnabled
		{
			get{return false;}
		}

		bool ILog.IsWarnEnabled
		{
			get{return false;}
		}

		bool ILog.IsErrorEnabled
		{
			get{return false;}
		}

		bool ILog.IsFatalEnabled
		{
			get{return false;}
		}

		void ILog.Debug(object message)
		{
		}

		void ILog.Debug(object message, Exception exception)
		{
		}

		void ILog.DebugFormat(string format, params object[] args)
		{
		}

		void ILog.DebugFormat(string format, object arg0)
		{
		}

		void ILog.DebugFormat(string format, object arg0, object arg1)
		{
		}

		void ILog.DebugFormat(string format, object arg0, object arg1, object arg2)
		{
		}

		void ILog.DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
		}

		void ILog.Info(object message)
		{
		}

		void ILog.Info(object message, Exception exception)
		{
		}

		void ILog.InfoFormat(string format, params object[] args)
		{
		}

		void ILog.InfoFormat(string format, object arg0)
		{
		}

		void ILog.InfoFormat(string format, object arg0, object arg1)
		{
		}

		void ILog.InfoFormat(string format, object arg0, object arg1, object arg2)
		{
		}

		void ILog.InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
		}

		void ILog.Warn(object message)
		{
		}

		void ILog.Warn(object message, Exception exception)
		{
		}

		void ILog.WarnFormat(string format, params object[] args)
		{
		}

		void ILog.WarnFormat(string format, object arg0)
		{
		}

		void ILog.WarnFormat(string format, object arg0, object arg1)
		{
		}

		void ILog.WarnFormat(string format, object arg0, object arg1, object arg2)
		{
		}

		void ILog.WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
		}

		void ILog.Error(object message)
		{
		}

		void ILog.Error(object message, Exception exception)
		{
		}

		void ILog.ErrorFormat(string format, params object[] args)
		{
		}

		void ILog.ErrorFormat(string format, object arg0)
		{
		}

		void ILog.ErrorFormat(string format, object arg0, object arg1)
		{
		}

		void ILog.ErrorFormat(string format, object arg0, object arg1, object arg2)
		{
		}

		void ILog.ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
		}

		void ILog.Fatal(object message)
		{
		}

		void ILog.Fatal(object message, Exception exception)
		{
		}

		void ILog.FatalFormat(string format, params object[] args)
		{
		}

		void ILog.FatalFormat(string format, object arg0)
		{
		}

		void ILog.FatalFormat(string format, object arg0, object arg1)
		{
		}

		void ILog.FatalFormat(string format, object arg0, object arg1, object arg2)
		{
		}

		void ILog.FatalFormat(IFormatProvider provider, string format, params object[] args)
		{
		}
	}
}
