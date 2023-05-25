using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Data
{
    /// <summary>
    /// Indicates that the instance can be converted to an ExpandoObject
    /// for use in a dynamic expression.
    /// </summary>
    public interface ISupportDynamic
    {
        public ExpandoObject MakeExpandoObject();
    }
}
