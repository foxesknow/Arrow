using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    public partial class DataBuffer<TData>
    {
        private struct ConditionState
        {
            public ConditionState(TaskCompletionSource<TData> tcs, Func<TData, bool> @if, Action<TData> then)
            {
                this.Tcs = tcs;
                this.IfCondition = @if;
                this.Then = then;
            }

            public void Deconstruct(out TaskCompletionSource<TData> tcs, out Func<TData, bool> @if, out Action<TData> then)
            {
                tcs = this.Tcs;
                @if = this.IfCondition;
                then = this.Then;
            }

            public TaskCompletionSource<TData> Tcs{get;}
            public Func<TData, bool> IfCondition{get;}
            public Action<TData> Then{get;}
        }

        private enum ExecutionOutcome
        {
            NotConsumed,
            Consumed
        }
    }
}
