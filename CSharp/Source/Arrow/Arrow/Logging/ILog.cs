using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.Logging
{
	/// <summary>
	/// Interface for all loggers, blatently "borrowed" from log4net
	/// </summary>
	public interface ILog
	{
		/// <summary>
		/// Writes a debug message
		/// </summary>
		/// <param name="message"></param>
		void Debug(object message);  
		
		/// <summary>
		/// Writes a debug message
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		void Debug(object message, Exception exception);
		
		/// <summary>
		/// Writes a debug message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void DebugFormat(string format, params object[] args); 
		
		/// <summary>
		/// Writes a debug message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		void DebugFormat(string format, object arg0); 
		
		/// <summary>
		/// Writes a debug message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		void DebugFormat(string format, object arg0, object arg1); 
		
		/// <summary>
		/// Writes a debug message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		void DebugFormat(string format, object arg0, object arg1, object arg2); 
		
		/// <summary>
		/// Writes a debug message
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void DebugFormat(IFormatProvider provider, string format, params object[] args);

		/// <summary>
		/// Writes an information message
		/// </summary>
		/// <param name="message"></param>
		void Info(object message);
		
		/// <summary>
		/// Writes an information message
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		void Info(object message, Exception exception);
		
		/// <summary>
		/// Writes an information message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void InfoFormat(string format, params object[] args);
		
		/// <summary>
		/// Writes an information message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		void InfoFormat(string format, object arg0); 
		
		/// <summary>
		/// Writes an information message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		void InfoFormat(string format, object arg0, object arg1); 
		
		/// <summary>
		/// Writes an information message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		void InfoFormat(string format, object arg0, object arg1, object arg2); 
		
		/// <summary>
		/// Writes an information message
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void InfoFormat(IFormatProvider provider, string format, params object[] args);
		
		/// <summary>
		/// Writes a warning message
		/// </summary>
		/// <param name="message"></param>
		void Warn(object message);
		
		/// <summary>
		/// Writes a warning message
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		void Warn(object message, Exception exception);
		
		/// <summary>
		/// Writes a warning message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void WarnFormat(string format, params object[] args);
		
		/// <summary>
		/// Writes a warning message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		void WarnFormat(string format, object arg0); 
		
		/// <summary>
		/// Writes a warning message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		void WarnFormat(string format, object arg0, object arg1); 
		
		/// <summary>
		/// Writes a warning message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		void WarnFormat(string format, object arg0, object arg1, object arg2); 
		
		/// <summary>
		/// Writes a warning message
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void WarnFormat(IFormatProvider provider, string format, params object[] args);

		/// <summary>
		/// Writes an error message
		/// </summary>
		/// <param name="message"></param>
		void Error(object message);
		
		/// <summary>
		/// Writes an error message
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		void Error(object message, Exception exception);
		
		/// <summary>
		/// Writes an error message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void ErrorFormat(string format, params object[] args);
		
		/// <summary>
		/// Writes an error message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		void ErrorFormat(string format, object arg0); 
		
		/// <summary>
		/// Writes an error message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		void ErrorFormat(string format, object arg0, object arg1); 
		
		/// <summary>
		/// Writes an error message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		void ErrorFormat(string format, object arg0, object arg1, object arg2); 
		
		/// <summary>
		/// Writes an error message
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void ErrorFormat(IFormatProvider provider, string format, params object[] args);
		
		/// <summary>
		/// Writes a fatal message
		/// </summary>
		/// <param name="message"></param>
		void Fatal(object message);
		
		/// <summary>
		/// Writes a fatal message
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		void Fatal(object message, Exception exception);
		
		/// <summary>
		/// Writes a fatal message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void FatalFormat(string format, params object[] args);
		
		/// <summary>
		/// Writes a fatal message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		void FatalFormat(string format, object arg0); 
		
		/// <summary>
		/// Writes a fatal message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		void FatalFormat(string format, object arg0, object arg1); 
		
		/// <summary>
		/// Writes a fatal message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		void FatalFormat(string format, object arg0, object arg1, object arg2); 
		
		/// <summary>
		/// Writes a fatal message
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void FatalFormat(IFormatProvider provider, string format, params object[] args);

		/// <summary>
		/// Indicates if debug logging is enabled
		/// </summary>
		bool IsDebugEnabled { get; }
		
		/// <summary>
		/// Indicates if information logging is enabled
		/// </summary>
		bool IsInfoEnabled { get; }
		
		/// <summary>
		/// Indicates if warning logging is enabled
		/// </summary>
		bool IsWarnEnabled { get; }
		
		/// <summary>
		/// Indicates if error logging is enabled
		/// </summary>
		bool IsErrorEnabled { get; }
		
		/// <summary>
		/// Indicates if fatal logging is enabled
		/// </summary>
		bool IsFatalEnabled { get; }
	}
}
