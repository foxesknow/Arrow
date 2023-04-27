using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Diagnostics;

using Arrow.Execution;

namespace Arrow.IO
{
    /// <summary>
    /// Creates a PID file that will be removed then the instance is disposed
    /// </summary>
    public sealed class PidFile : IDisposable
    {
        private readonly string m_Filename;
        private bool m_Disposed;

        /// <summary>
        /// Creates the pid file, setting its contents to the process id of the host process.
        /// </summary>
        /// <param name="filename"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public PidFile(string filename)
        {
            if(filename is null) throw new ArgumentNullException(nameof(filename));
            if(string.IsNullOrWhiteSpace(filename)) throw new ArgumentException("invalid filename", nameof(filename));

            m_Filename = filename;

            var pid = GetProcessID();
            File.WriteAllText(filename, pid.ToString());
        }

        /// <summary>
        /// Deletes the PID file
        /// </summary>
        public void Dispose()
        {
            if(m_Disposed == false)
            {
                MethodCall.AllowFail(m_Filename, static filename => File.Delete(filename));
                m_Disposed = true;
            }
        }

        private int GetProcessID()
        {
            return Process.GetCurrentProcess().Id;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return m_Filename;
        }
    }
}
