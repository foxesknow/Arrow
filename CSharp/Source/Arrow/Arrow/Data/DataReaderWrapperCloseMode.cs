using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Data
{
    public enum DataReaderWrapperCloseMode
    {
        /// <summary>
        /// Closes nothing
        /// </summary>
        None,

        /// <summary>
        /// Closes the reader
        /// </summary>
        Reader,

        /// <summary>
        /// Closes the reader and the command attached to it
        /// </summary>
        Command,

        /// <summary>
        /// Closes, the reader, the command and the connection
        /// </summary>
        Connection
    }
}
