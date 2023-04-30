using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Logging;

namespace Tango.Workbench.Python
{
    /// <summary>
    /// IronPython has a problem with explicitly implemented interfaces so
    /// we'll define a much simplified logging interface
    /// </summary>
    public sealed class PythonLogger
    {
        private readonly ILog m_Log;

        public PythonLogger(ILog log)
        {
            m_Log = log;
        }

        public void Debug(string message)
        {
            m_Log.Debug(message);
        }

        public void Info(string message)
        {
            m_Log.Info(message);
        }

        public void Warn(string message)
        {
            m_Log.Warn(message);
        }

        public void Error(string message)
        {
            m_Log.Error(message);
        }

        public void Fatal(string message)
        {
            m_Log.Fatal(message);
        }
    }
}
