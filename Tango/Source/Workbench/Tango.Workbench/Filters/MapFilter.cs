using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Arrow.Scripting.Wire.StaticExpression;

namespace Tango.Workbench.Filters
{
    [Filter("Map")]
    public sealed class MapFilter : Filter
    {
        private static readonly Func<object, long, object?> AlwaysNull = static (_, _) => null;

        private readonly ExpressionCompiler<object?> m_Expressions = new(AlwaysNull);

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

        /// <summary>
        /// The transformation that will be applied
        /// </summary>
        public string? Transformation{get; set;}
    }
}
