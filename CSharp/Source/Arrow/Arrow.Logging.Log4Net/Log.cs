using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Arrow.Configuration;
using Arrow.Text;
using Arrow.Storage;

namespace Arrow.Logging.Log4Net
{
	/// <summary>
	/// Log class that uses the Apache log4net framework
	/// </summary>
	public class Log : ILogInitializer, ILog
	{
		private log4net.ILog m_Log;
	
		/// <summary>
		/// Initializes log4net
		/// </summary>
		static Log()
		{
			try
			{
				XmlNode configurationNode=AppConfig.GetSectionXml(ArrowSystem.Name,"Arrow.Logging.Log4Net/Configuration");
				if(configurationNode!=null)
				{
					// We've got an explicit path to log4net configuration file
					string path=configurationNode.InnerText;
					path=TokenExpander.ExpandText(path);
					Uri uri=Accessor.CreateUri(path);
					
					XmlDocument doc=StorageManager.Get(uri).ReadXmlDocument();
					log4net.Config.XmlConfigurator.Configure(doc.DocumentElement);
				}
				else
				{
					// Assume the config is in the app.config
					log4net.Config.XmlConfigurator.Configure();
				}
			
			}
			catch
			{
				log4net.Config.XmlConfigurator.Configure();
			}
		}

		#region ILogInitializer Members

		void ILogInitializer.Initialize(string name)
		{
			m_Log=log4net.LogManager.GetLogger(name ?? "root");
		}

		#endregion

		#region ILog Members

		void ILog.Debug(object message)
		{
			m_Log.Debug(message);
		}

		void ILog.Debug(object message, Exception exception)
		{
			m_Log.Debug(message,exception);
		}

		void ILog.DebugFormat(string format, params object[] args)
		{
			m_Log.DebugFormat(format,args);
		}

		void ILog.DebugFormat(string format, object arg0)
		{
			m_Log.DebugFormat(format,arg0);
		}

		void ILog.DebugFormat(string format, object arg0, object arg1)
		{
			m_Log.DebugFormat(format,arg0,arg1);
		}

		void ILog.DebugFormat(string format, object arg0, object arg1, object arg2)
		{
			m_Log.DebugFormat(format,arg0,arg1,arg2);
		}

		void ILog.DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			m_Log.DebugFormat(provider,format,args);
		}

		void ILog.Info(object message)
		{
			m_Log.Info(message);
		}

		void ILog.Info(object message, Exception exception)
		{
			m_Log.Info(message,exception);
		}

		void ILog.InfoFormat(string format, params object[] args)
		{
			m_Log.InfoFormat(format,args);
		}

		void ILog.InfoFormat(string format, object arg0)
		{
			m_Log.InfoFormat(format,arg0);
		}

		void ILog.InfoFormat(string format, object arg0, object arg1)
		{
			m_Log.InfoFormat(format,arg0,arg1);
		}

		void ILog.InfoFormat(string format, object arg0, object arg1, object arg2)
		{
			m_Log.InfoFormat(format,arg0,arg1,arg2);
		}

		void ILog.InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			m_Log.InfoFormat(provider,format,args);
		}

		void ILog.Warn(object message)
		{
			m_Log.Warn(message);
		}

		void ILog.Warn(object message, Exception exception)
		{
			m_Log.Warn(message,exception);
		}

		void ILog.WarnFormat(string format, params object[] args)
		{
			m_Log.WarnFormat(format,args);
		}

		void ILog.WarnFormat(string format, object arg0)
		{
			m_Log.WarnFormat(format,arg0);
		}

		void ILog.WarnFormat(string format, object arg0, object arg1)
		{
			m_Log.WarnFormat(format,arg0,arg1);
		}

		void ILog.WarnFormat(string format, object arg0, object arg1, object arg2)
		{
			m_Log.WarnFormat(format,arg0,arg1,arg2);
		}

		void ILog.WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			m_Log.WarnFormat(provider,format,args);
		}

		void ILog.Error(object message)
		{
			m_Log.Error(message);
		}

		void ILog.Error(object message, Exception exception)
		{
			m_Log.Error(message,exception);
		}

		void ILog.ErrorFormat(string format, params object[] args)
		{
			m_Log.ErrorFormat(format,args);
		}

		void ILog.ErrorFormat(string format, object arg0)
		{
			m_Log.ErrorFormat(format,arg0);
		}

		void ILog.ErrorFormat(string format, object arg0, object arg1)
		{
			m_Log.ErrorFormat(format,arg0,arg1);
		}

		void ILog.ErrorFormat(string format, object arg0, object arg1, object arg2)
		{
			m_Log.ErrorFormat(format,arg0,arg1,arg2);
		}

		void ILog.ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			m_Log.ErrorFormat(provider,format,args);
		}

		void ILog.Fatal(object message)
		{
			m_Log.Fatal(message);
		}

		void ILog.Fatal(object message, Exception exception)
		{
			m_Log.Fatal(message,exception);
		}

		void ILog.FatalFormat(string format, params object[] args)
		{
			m_Log.FatalFormat(format,args);
		}

		void ILog.FatalFormat(string format, object arg0)
		{
			m_Log.FatalFormat(format,arg0);
		}

		void ILog.FatalFormat(string format, object arg0, object arg1)
		{
			m_Log.FatalFormat(format,arg0,arg1);
		}

		void ILog.FatalFormat(string format, object arg0, object arg1, object arg2)
		{
			m_Log.FatalFormat(format,arg0,arg1,arg2);
		}

		void ILog.FatalFormat(IFormatProvider provider, string format, params object[] args)
		{
			m_Log.FatalFormat(provider,format,args);
		}

		bool ILog.IsDebugEnabled
		{
			get{return m_Log.IsDebugEnabled;}
		}

		bool ILog.IsInfoEnabled
		{
			get{return m_Log.IsInfoEnabled;}
		}

		bool ILog.IsWarnEnabled
		{
			get{return m_Log.IsWarnEnabled;}
		}

		bool ILog.IsErrorEnabled
		{
			get{return m_Log.IsErrorEnabled;}
		}

		bool ILog.IsFatalEnabled
		{
			get{return m_Log.IsFatalEnabled;}
		}

		#endregion
	}
}
