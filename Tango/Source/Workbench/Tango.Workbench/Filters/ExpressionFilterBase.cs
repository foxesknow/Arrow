using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Scripting.Wire.StaticExpression;

namespace Tango.Workbench.Filters
{
    public abstract class ExpressionFilterBase<TResult> : Filter
    {
        private readonly Dictionary<Type, Func<object, long, TResult>> m_Cache = new();

        /// <summary>
        /// Fetches the function for the given type, compiling the script on demand if required.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        protected Func<object, long, TResult> GetFunction(string script, Type itemType)
        {
            if(m_Cache.TryGetValue(itemType, out var function) == false)
            {
                function = Compile(script, itemType);
                m_Cache.Add(itemType, function);
            }

            return function;
        }

        /// <summary>
        /// Creates a default context containing useful references
        /// </summary>
        /// <returns></returns>
        protected StaticParseContext MakeParseContext()
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

        /// <summary>
        /// Compiles the script into a function
        /// </summary>
        /// <param name="script"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        protected abstract Func<object, long, TResult> Compile(string script, Type itemType);
    }
}
