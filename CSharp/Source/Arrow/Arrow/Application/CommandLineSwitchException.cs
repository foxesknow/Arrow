using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.Application
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CommandLineSwitchException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public CommandLineSwitchException() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public CommandLineSwitchException(string message) : base(message) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public CommandLineSwitchException(string message, Exception inner) : base(message, inner) { }
    }
}
