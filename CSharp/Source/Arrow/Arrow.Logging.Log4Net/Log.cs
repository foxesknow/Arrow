﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Arrow.Configuration;
using Arrow.Text;
using Arrow.Storage;
using log4net;

namespace Arrow.Logging.Log4Net
{
    /// <summary>
    /// Log class that uses the Apache log4net framework
    /// </summary>
    public class Log : ILogInitializer, ILog, IPropertyContext, IForContext
    {
        private static readonly AsyncPropertyPusher s_PropertyPusher = new();

        private log4net.ILog m_Log;

        /// <summary>
        /// Initializes log4net
        /// </summary>
        static Log()
        {
            try
            {
                var config = AppConfig.GetSectionObject<Config>(ArrowSystem.Name, "Arrow.Logging.Log4Net");
                if(config is not null)
                {
                    // We've got an explicit path to log4net configuration file
                    string path = config.Configuration;
                    path = TokenExpander.ExpandText(path);
                    Uri uri = Accessor.CreateUri(path);

                    XmlDocument doc = StorageManager.Get(uri).ReadXmlDocument();
                    var expandedXml = TokenExpander.ExpandText(doc.OuterXml, "$(", ")");

                    var expandedDoc = new XmlDocument();
                    expandedDoc.LoadXml(expandedXml);

                    log4net.Config.XmlConfigurator.Configure(expandedDoc.DocumentElement);
                }
                else
                {
                    // Assume the config is in the app.config
                    log4net.Config.XmlConfigurator.Configure();
                }

            }
            catch
            {
                // If all goes wrong then try to do something!
                log4net.Config.XmlConfigurator.Configure();
            }

            // We don't need this
            GlobalContext.Properties.Remove(log4net.Core.LoggingEvent.HostNameProperty);
        }

        private Log(log4net.ILog log)
        {
            m_Log = log;
        }

        void ILogInitializer.Initialize(string name)
        {
            m_Log = log4net.LogManager.GetLogger(name ?? "root");
        }

        void ILog.Debug(object message)
        {
            m_Log.Debug(message);
        }

        void ILog.Debug(object message, Exception exception)
        {
            m_Log.Debug(message, exception);
        }

        void ILog.DebugFormat(string format, params object[] args)
        {
            m_Log.DebugFormat(format, args);
        }

        void ILog.DebugFormat(string format, object arg0)
        {
            m_Log.DebugFormat(format, arg0);
        }

        void ILog.DebugFormat(string format, object arg0, object arg1)
        {
            m_Log.DebugFormat(format, arg0, arg1);
        }

        void ILog.DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            m_Log.DebugFormat(format, arg0, arg1, arg2);
        }

        void ILog.DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            m_Log.DebugFormat(provider, format, args);
        }

        void ILog.Info(object message)
        {
            m_Log.Info(message);
        }

        void ILog.Info(object message, Exception exception)
        {
            m_Log.Info(message, exception);
        }

        void ILog.InfoFormat(string format, params object[] args)
        {
            m_Log.InfoFormat(format, args);
        }

        void ILog.InfoFormat(string format, object arg0)
        {
            m_Log.InfoFormat(format, arg0);
        }

        void ILog.InfoFormat(string format, object arg0, object arg1)
        {
            m_Log.InfoFormat(format, arg0, arg1);
        }

        void ILog.InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            m_Log.InfoFormat(format, arg0, arg1, arg2);
        }

        void ILog.InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            m_Log.InfoFormat(provider, format, args);
        }

        void ILog.Warn(object message)
        {
            m_Log.Warn(message);
        }

        void ILog.Warn(object message, Exception exception)
        {
            m_Log.Warn(message, exception);
        }

        void ILog.WarnFormat(string format, params object[] args)
        {
            m_Log.WarnFormat(format, args);
        }

        void ILog.WarnFormat(string format, object arg0)
        {
            m_Log.WarnFormat(format, arg0);
        }

        void ILog.WarnFormat(string format, object arg0, object arg1)
        {
            m_Log.WarnFormat(format, arg0, arg1);
        }

        void ILog.WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            m_Log.WarnFormat(format, arg0, arg1, arg2);
        }

        void ILog.WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            m_Log.WarnFormat(provider, format, args);
        }

        void ILog.Error(object message)
        {
            m_Log.Error(message);
        }

        void ILog.Error(object message, Exception exception)
        {
            m_Log.Error(message, exception);
        }

        void ILog.ErrorFormat(string format, params object[] args)
        {
            m_Log.ErrorFormat(format, args);
        }

        void ILog.ErrorFormat(string format, object arg0)
        {
            m_Log.ErrorFormat(format, arg0);
        }

        void ILog.ErrorFormat(string format, object arg0, object arg1)
        {
            m_Log.ErrorFormat(format, arg0, arg1);
        }

        void ILog.ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            m_Log.ErrorFormat(format, arg0, arg1, arg2);
        }

        void ILog.ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            m_Log.ErrorFormat(provider, format, args);
        }

        void ILog.Fatal(object message)
        {
            m_Log.Fatal(message);
        }

        void ILog.Fatal(object message, Exception exception)
        {
            m_Log.Fatal(message, exception);
        }

        void ILog.FatalFormat(string format, params object[] args)
        {
            m_Log.FatalFormat(format, args);
        }

        void ILog.FatalFormat(string format, object arg0)
        {
            m_Log.FatalFormat(format, arg0);
        }

        void ILog.FatalFormat(string format, object arg0, object arg1)
        {
            m_Log.FatalFormat(format, arg0, arg1);
        }

        void ILog.FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            m_Log.FatalFormat(format, arg0, arg1, arg2);
        }

        void ILog.FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            m_Log.FatalFormat(provider, format, args);
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

		IPropertyPusher IPropertyContext.GetPusher()
		{
			return s_PropertyPusher;
		}

		ILog IForContext.ForContext<T>()
		{
			IForContext self = this;
			return self.ForContext(typeof(T));
		}

		ILog IForContext.ForContext(Type type)
		{
			if(type is null) return this;

			var log = log4net.LogManager.GetLogger(type);
			if(log is null) return this;

			return new Log(log);
		}

		ILog IForContext.ForContext(string name)
		{
			if(name is null) return this;

			var log = log4net.LogManager.GetLogger(name);
			if(log is null) return this;

			return new Log(log);
		}

		class Config
		{
			public string Configuration{get; set;}
		}
	}
}
