using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Execution
{
    /// <summary>
    /// Allows you to access a wrapped item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IWrapper<out T>
    {
        /// <summary>
        /// Returns the wrapped item
        /// </summary>
        public T WrappedItem{get;}
    }
}
