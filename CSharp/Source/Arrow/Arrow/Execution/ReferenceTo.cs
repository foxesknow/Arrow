using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Execution
{
    /// <summary>
    /// Acts as a variable that allows a value to be passed to methods that do not
    /// support out or ref parameters, such as aync methods
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ReferenceTo<T>
    {
        /// <summary>
        /// Initializes the instance
        /// </summary>
        public ReferenceTo()
        {
            this.Value = default!;
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="value"></param>
        public ReferenceTo(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// The value being referenced
        /// </summary>
        public T Value{get; set;}

        /// <inheritdoc/>
        public override string ToString()
        {
            if(this.Value is null)
            {
                return "null";
            }
            else
            {
                return this.Value.ToString() ?? "";
            }
        }
    }
}
