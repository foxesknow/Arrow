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
    /// Maps an incoming item to a new item.
    /// If the new item is null then it is not passed through the stream.
    /// </summary>
    [Filter("Map")]
    [Filter("Select")]
    public sealed class MapFilter : Filter, ISupportInitialize
    {
        private readonly ExpressionCompiler<object?> m_Expressions = new(ExpressionCompiler.AlwaysNull);

        public override async IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            if(this.Transformation is null) throw new ArgumentNullException(nameof(Transformation));

            long index = 0;

            await foreach(var item in items)
            {
                var itemType = item.GetType();
                var function = m_Expressions.GetFunction(this.Transformation, itemType, this.Log);

                var newItem = function(item, index++);
                if(newItem is not null) yield return newItem;
            }
        }

        void ISupportInitialize.BeginInit()
        {
        }

        void ISupportInitialize.EndInit()
        {
            if(string.IsNullOrWhiteSpace(this.Transformation)) throw new WorkbenchException("invalid transformation");
        }

        /// <summary>
        /// The transformation that will be applied
        /// </summary>
        public string? Transformation{get; set;}
    }
}
