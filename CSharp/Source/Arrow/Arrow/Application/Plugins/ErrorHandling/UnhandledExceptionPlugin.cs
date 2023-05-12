using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Logging;

#nullable disable

namespace Arrow.Application.Plugins.ErrorHandling
{
	/// <summary>
	/// Listens out for unhandled exceptions in the current app domain and logs them
	/// </summary>
	public class UnhandledExceptionPlugin : Plugin
	{
		private ILog m_Log;
		
		/// <summary>
		/// The name of the log to write to
		/// </summary>
		public string LogName{get; set;}

        /// <summary>
        /// Starts the plugin
        /// </summary>
        protected internal override ValueTask Start()
        {
            m_Log = LogManager.GetLog(this.LogName);
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            return default;
        }

        /// <summary>
        /// Stops the plugin
        /// </summary>
        protected internal override ValueTask Stop()
        {
            AppDomain.CurrentDomain.UnhandledException -= UnhandledException;

            return default;
        }

        /// <summary>
        /// Returns the name of the plugin
        /// </summary>
        public override string Name
        {
            get{return "UnhandledExceptionPlugin";}
        }

        private void UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            string exception = "";
            if(args.ExceptionObject != null) exception = args.ExceptionObject.ToString();

            m_Log.ErrorFormat("UnhandledException - IsTerminating={0}, Exception={1}", args.IsTerminating, exception);
        }
    }
}
