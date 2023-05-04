using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Scripting.Wire.StaticExpression;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Base class for a filter expression compiler
    /// </summary>
    public abstract class ExpressionCompiler
    {
        public static readonly Func<object, long, object?> AlwaysNull = static (_, _) => null;

        public static readonly Func<object, long, bool> AlwaysFalse = static (_, _) => false;

        public static readonly Func<object, long, bool> AlwaysTrue = static (_, _) => true;
        
        /// <summary>
        /// Creates a default context containing useful references
        /// </summary>
        /// <returns></returns>
        protected virtual StaticParseContext MakeParseContext()
        {
            var parseContext = new StaticParseContext();
            parseContext.References.Add(typeof(string).Assembly);
            parseContext.Usings.Add("System");

            parseContext.References.Add(typeof(System.IO.Path).Assembly);
            parseContext.Usings.Add("System.IO");

            parseContext.References.Add(typeof(Arrow.Calendar.Clock).Assembly);
            parseContext.Usings.Add("Arrow.Calendar");

            return parseContext;
        }
    }
}
