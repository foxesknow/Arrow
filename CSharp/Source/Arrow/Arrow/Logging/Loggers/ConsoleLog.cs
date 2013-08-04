using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Arrow.Execution;

namespace Arrow.Logging.Loggers
{
	/// <summary>
	/// Writes to the console window.
	/// Output is sent to Console.Error so that it will not mix with redirected output
	/// </summary>
	public class ConsoleLog : ILog
	{
		private static readonly object s_Sync=new object();
		
		private TextWriter m_Out=Console.Error;
		
		private ConsoleColor m_DebugColor=ConsoleColor.Gray;
		private ConsoleColor m_InfoColor=ConsoleColor.White;
		private ConsoleColor m_WarnColor=ConsoleColor.DarkYellow;
		private ConsoleColor m_ErrorColor=ConsoleColor.Magenta;
		private ConsoleColor m_FatalColor=ConsoleColor.Red;
		
		private bool m_DebugEnabled=true;
		private bool m_InfoEnabled=true;
		private bool m_WarnEnabled=true;
		private bool m_ErrorEnabled=true;
		private bool m_FatalEnabled=true;
		
		/// <summary>
		/// Enables/disables debug logging
		/// </summary>
		public bool DebugEnabled
		{
			set{m_DebugEnabled=value;}
		}
		
		/// <summary>
		/// Enables/disables info logging
		/// </summary>
		public bool InfoEnabled
		{
			set{m_InfoEnabled=value;}
		}
		
		/// <summary>
		/// Enables/disables warning logging
		/// </summary>
		public bool WarnEnabled
		{
			set{m_WarnEnabled=value;}
		}
		
		/// <summary>
		/// Enables/disables error logging
		/// </summary>
		public bool ErrorEnabled
		{
			set{m_ErrorEnabled=value;}
		}
		
		/// <summary>
		/// Enables/disables fatal logging
		/// </summary>
		public bool FatalEnabled
		{
			set{m_FatalEnabled=value;}
		}
		
		private void Log(ConsoleColor color, object message)
		{
			MethodCall.AllowFail(delegate
			{
				lock(s_Sync)
				using(new ColorChange(color))
				{
					m_Out.WriteLine(message);
				}
			});
		}
		
		private void Log(ConsoleColor color, object message, Exception exception)
		{
			MethodCall.AllowFail(delegate
			{
				lock(s_Sync)
				using(new ColorChange(color))
				{
					m_Out.WriteLine(message);
					m_Out.WriteLine(exception);
				}
			});
		}
		
		private void Log(ConsoleColor color, string format, params object[] args)
		{
			MethodCall.AllowFail(delegate
			{
				lock(s_Sync)
				using(new ColorChange(color))
				{
					m_Out.WriteLine(format,args);
				}
			});
		}
		
		private void Log(ConsoleColor color, IFormatProvider provider, string format, params object[] args)
		{
			MethodCall.AllowFail(delegate
			{
				lock(s_Sync)
				using(new ColorChange(color))
				{
					string message=string.Format(provider,format,args);
					m_Out.WriteLine(message);
				}
			});
		}
		
		#region ILog Members

		void ILog.Debug(object message)
		{
			if(m_DebugEnabled) Log(m_DebugColor,message);
		}

		void ILog.Debug(object message, Exception exception)
		{
			if(m_DebugEnabled) Log(m_DebugColor,message,exception);
		}

		void ILog.DebugFormat(string format, params object[] args)
		{
			if(m_DebugEnabled) Log(m_DebugColor,format,args);
		}

		void ILog.DebugFormat(string format, object arg0)
		{
			if(m_DebugEnabled) Log(m_DebugColor,format,arg0);
		}

		void ILog.DebugFormat(string format, object arg0, object arg1)
		{
			if(m_DebugEnabled) Log(m_DebugColor,format,arg0,arg1);
		}

		void ILog.DebugFormat(string format, object arg0, object arg1, object arg2)
		{
			if(m_DebugEnabled) Log(m_DebugColor,format,arg0,arg1,arg2);
		}

		void ILog.DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			if(m_DebugEnabled) Log(m_DebugColor,provider,format,args);
		}

		void ILog.Info(object message)
		{
			if(m_InfoEnabled) Log(m_InfoColor,message);
		}

		void ILog.Info(object message, Exception exception)
		{
			if(m_InfoEnabled) Log(m_InfoColor,message,exception);
		}

		void ILog.InfoFormat(string format, params object[] args)
		{
			if(m_InfoEnabled) Log(m_InfoColor,format,args);
		}

		void ILog.InfoFormat(string format, object arg0)
		{
			if(m_InfoEnabled) Log(m_InfoColor,format,arg0);
		}

		void ILog.InfoFormat(string format, object arg0, object arg1)
		{
			if(m_InfoEnabled) Log(m_InfoColor,format,arg0,arg1);
		}

		void ILog.InfoFormat(string format, object arg0, object arg1, object arg2)
		{
			if(m_InfoEnabled) Log(m_InfoColor,format,arg0,arg1,arg2);
		}

		void ILog.InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			if(m_InfoEnabled) Log(m_InfoColor,provider,format,args);
		}

		void ILog.Warn(object message)
		{
			if(m_WarnEnabled) Log(m_WarnColor,message);
		}

		void ILog.Warn(object message, Exception exception)
		{
			if(m_WarnEnabled) Log(m_WarnColor,message,exception);
		}

		void ILog.WarnFormat(string format, params object[] args)
		{
			if(m_WarnEnabled) Log(m_WarnColor,format,args);
		}

		void ILog.WarnFormat(string format, object arg0)
		{
			if(m_WarnEnabled) Log(m_WarnColor,format,arg0);
		}

		void ILog.WarnFormat(string format, object arg0, object arg1)
		{
			if(m_WarnEnabled) Log(m_WarnColor,format,arg0,arg1);
		}

		void ILog.WarnFormat(string format, object arg0, object arg1, object arg2)
		{
			if(m_WarnEnabled) Log(m_WarnColor,format,arg0,arg1,arg2);
		}

		void ILog.WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			if(m_WarnEnabled) Log(m_WarnColor,provider,format,args);
		}

		void ILog.Error(object message)
		{
			if(m_ErrorEnabled) Log(m_ErrorColor,message);
		}

		void ILog.Error(object message, Exception exception)
		{
			if(m_ErrorEnabled) Log(m_ErrorColor,message,exception);
		}

		void ILog.ErrorFormat(string format, params object[] args)
		{
			if(m_ErrorEnabled) Log(m_ErrorColor,format,args);
		}

		void ILog.ErrorFormat(string format, object arg0)
		{
			if(m_ErrorEnabled) Log(m_ErrorColor,format,arg0);
		}

		void ILog.ErrorFormat(string format, object arg0, object arg1)
		{
			if(m_ErrorEnabled) Log(m_ErrorColor,format,arg0,arg1);
		}

		void ILog.ErrorFormat(string format, object arg0, object arg1, object arg2)
		{
			if(m_ErrorEnabled) Log(m_ErrorColor,format,arg0,arg1,arg2);
		}

		void ILog.ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			if(m_ErrorEnabled) Log(m_ErrorColor,provider,format,args);
		}

		void ILog.Fatal(object message)
		{
			if(m_FatalEnabled) Log(m_FatalColor,message);
		}

		void ILog.Fatal(object message, Exception exception)
		{
			if(m_FatalEnabled) Log(m_FatalColor,message,exception);
		}

		void ILog.FatalFormat(string format, params object[] args)
		{
			if(m_FatalEnabled) Log(m_FatalColor,format,args);
		}

		void ILog.FatalFormat(string format, object arg0)
		{
			if(m_FatalEnabled) Log(m_FatalColor,format,arg0);
		}

		void ILog.FatalFormat(string format, object arg0, object arg1)
		{
			if(m_FatalEnabled) Log(m_FatalColor,format,arg0,arg1);
		}

		void ILog.FatalFormat(string format, object arg0, object arg1, object arg2)
		{
			if(m_FatalEnabled) Log(m_FatalColor,format,arg0,arg1,arg2);
		}

		void ILog.FatalFormat(IFormatProvider provider, string format, params object[] args)
		{
			if(m_FatalEnabled) Log(m_FatalColor,provider,format,args);
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

		#endregion
		
		struct ColorChange : IDisposable
		{
			ConsoleColor m_Fore;
			
			public ColorChange(ConsoleColor fore)
			{
				m_Fore=fore;
				Console.ForegroundColor=fore;
			}

			#region IDisposable Members

			public void Dispose()
			{
				Console.ResetColor();
			}

			#endregion
		}
	}
}
