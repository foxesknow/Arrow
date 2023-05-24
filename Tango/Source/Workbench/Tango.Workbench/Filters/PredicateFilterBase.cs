using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public abstract class PredicateFilterBase : Filter, ISupportInitialize
    {
        private readonly StaticExpressionCompiler<bool> m_Predicates = new(StaticExpressionCompiler.AlwaysFalse);

        protected Func<object, long, bool> GetFunction(Type itemType)
        {
            return m_Predicates.GetFunction(this.Predicate!, itemType, this.Log);
        }

        void ISupportInitialize.BeginInit()
        {
        }

        void ISupportInitialize.EndInit()
        {
            if(string.IsNullOrWhiteSpace(this.Predicate)) throw new WorkbenchException("invalid predicate");
        }

        /// <summary>
        /// The predicate to evaluate
        /// </summary>
        public string? Predicate{get; set;}
    }
}
