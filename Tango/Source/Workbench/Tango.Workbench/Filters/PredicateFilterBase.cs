using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Arrow.Scripting.Wire.StaticExpression;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Base class for all filters that preform some sort of filtering.
    /// 
    /// The predicate that is compiles has an "item" and "index" parameter.
    /// "item" is the item going through the pipeline
    /// "index" is the 0-based position of the item in the stream
    /// </summary>
    public abstract class PredicateFilterBase : Filter
    {
        private static readonly Func<object, long, bool> AlwaysFalse = static (_, _) => false;

        private readonly ExpressionCompiler<bool> m_Predicates = new(AlwaysFalse);

        protected Func<object, long, bool> GetFunction(string script, Type itemType)
        {
            return m_Predicates.GetFunction(script, itemType, this.Log);
        }        
    }
}
