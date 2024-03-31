using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Reflection
{
    /// <summary>
    /// A value that can be used to pass as a ref parameter in a lambda
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class DiscardableRef<T>
    {
        /// <summary>
        /// The default value for T
        /// </summary>
        public static T Value = default!;

        /// <summary>
        /// A reference to a variable of type T
        /// </summary>
        public static ref T RefValue
        {
            get{return ref Value;}
        }
    }
}
