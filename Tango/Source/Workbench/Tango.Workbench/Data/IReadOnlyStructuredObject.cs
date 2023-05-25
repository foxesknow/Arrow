using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Data
{
    public interface IReadOnlyStructuredObject : IEnumerable<KeyValuePair<string, object?>>
    {
        /// <summary>
        /// Returns the value of the named item
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object? this[string name]{get;}
        
        /// <summary>
        /// Returns the value at the given indec
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object? this[int index]{get;}

        /// <summary>
        /// How many items are in the structured object
        /// </summary>
        public int Count{get;}

        /// <summary>
        /// Attempts to get a value from the object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string name, out object? value);
    }
}
