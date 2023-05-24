using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Logging;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Applied a series of case statements until one of their "If" predicate is true, 
    /// at which point it executes the "Then" function to transform the item.
    /// 
    /// If a transformation returns null then the item is not passed through the filter.
    /// </summary>
    [Filter("Switch")]
    public sealed class SwitchFilter : Filter
    {
        private readonly StaticExpressionCompiler<bool> m_IfConditions = new();
        private readonly StaticExpressionCompiler<object?> m_Thens = new();

        public override async IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            long index = 0;

            await foreach(var item in items)
            {
                var itemType = item.GetType();

                var matched = false;
                for(int i = 0; i < this.Cases.Count && !matched; i++)
                {
                    var @case = this.Cases[i];

                    var condition = m_IfConditions.GetFunction(@case.If!, itemType, this.Log);
                    if(condition(item, index))
                    {
                        matched = true;

                        var then = m_Thens.GetFunction(@case.Then!, item.GetType(), this.Log);
                        var newItem = @then(item, index);
                        
                        if(newItem is not null) yield return newItem;
                        break;
                    }
                }

                if(matched == false)
                {
                    VerboseLog.Warn("no cases matched");
                }

                index++;
            }
        }

        

        public List<Case> Cases{get;} = new();

        public class Case : ISupportInitialize
        {
            /// <summary>
            /// The condition to evaulate. If true then the "Then" expression is run
            /// </summary>
            public string? If{get; set;}

            /// <summary>
            /// Run if the "If" expression is true, and can be used to transform the item
            /// </summary>
            public string? Then{get; set;}

            void ISupportInitialize.BeginInit()
            {
            }

            void ISupportInitialize.EndInit()
            {
                var invalidCase = string.IsNullOrWhiteSpace(this.If) || string.IsNullOrWhiteSpace(this.Then);
                if(invalidCase)
                {
                    throw new WorkbenchException("invalid case clause");
                }
            }
        }
    }
}
