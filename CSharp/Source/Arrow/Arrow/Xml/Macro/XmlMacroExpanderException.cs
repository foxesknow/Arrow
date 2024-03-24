using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.Xml.Macro
{
    /// <summary>
    /// Base class for macro expansion exceptions
    /// </summary>
    [Serializable]
    public class XmlMacroExpanderException : ArrowException
    {
        /// <summary>
        /// 
        /// </summary>
        public XmlMacroExpanderException() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public XmlMacroExpanderException(string message) : base(message) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public XmlMacroExpanderException(string message, Exception inner) : base(message, inner) { }
    }
}
