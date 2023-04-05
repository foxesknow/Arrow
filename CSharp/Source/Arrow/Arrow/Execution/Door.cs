using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Execution
{
    /// <summary>
    /// A door is a value that can only be open or closed
    /// When the door is closed nothing else can go through until the door is opened.
    /// This is useful for handling multiple callbacks where you only want to process once.
    /// </summary>
    public sealed class Door
    {
        private long m_IsClosed;

        private readonly Action m_Releaser;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        public Door()
        {
            m_Releaser = Release;
        }

        /// <summary>
        /// Attempts to close the door
        /// </summary>
        /// <param name="releaser">On success an object that will open the door when disposed</param>
        /// <returns>true if the door was closed, otherwise false</returns>
        public bool TryClose([NotNullWhen(true)]out IDisposable? releaser)
        {
            if(Interlocked.CompareExchange(ref m_IsClosed, 1, 0) == 0)
            {
               releaser = new Disposer(m_Releaser);
               return true;
            }

            releaser = null;
            return false;
        }

        /// <summary>
        /// Checks to see if the door is closed
        /// </summary>
        public bool IsClosed
        {
            get{return Interlocked.Read(ref m_IsClosed) == 1;}
        }

        /// <summary>
        /// Checks to see if the door is open
        /// </summary>
        public bool IsOpen
        {
            get{return Interlocked.Read(ref m_IsClosed) == 0;}
        }

        private void Release()
        {
            Interlocked.Exchange(ref m_IsClosed, 0);
        }
    }
}
