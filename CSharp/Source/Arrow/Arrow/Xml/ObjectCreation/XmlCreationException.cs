using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Xml.ObjectCreation
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class XmlCreationException : ArrowException
    {
        /// <summary>
        /// 
        /// </summary>
        public XmlCreationException() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public XmlCreationException(string message) : base(message) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public XmlCreationException(string message, Exception inner) : base(message, inner) { }
    }
}
