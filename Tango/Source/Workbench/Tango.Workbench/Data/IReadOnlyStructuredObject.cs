using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Data
{
    public interface IReadOnlyStructuredObject : IEnumerable<KeyValuePair<string, object?>>
    {
        public object? this[string name]{get;}

        public bool TryGetValue(string name, out object? value);
    }
}
