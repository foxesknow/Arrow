using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Execution
{
    public sealed class AsyncDisposer : IAsyncDisposable
    {
        private Func<ValueTask>? m_Function;

        public AsyncDisposer(Func<ValueTask> function)
        {
            if(function is null) throw new ArgumentNullException(nameof(function));

            m_Function = function;
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if(m_Function is not null)
            {
                await m_Function();
                m_Function = null;
            }
        }

        /// <summary>
		/// Creates an instance
		/// </summary>
		/// <param name="function"></param>
		/// <returns></returns>
		public static IAsyncDisposable Make(Func<ValueTask> function)
		{
			return new AsyncDisposer(function);
		}

        /// <summary>
		/// Creates an instance
		/// </summary>
		/// <param name="function"></param>
		/// <returns></returns>
		public static IAsyncDisposable Make(Action action)
		{
            if(action is null) throw new ArgumentNullException(nameof(action));

            Func<ValueTask> function = () =>
            {
                action();
                return default;
            };

			return new AsyncDisposer(function);
		}
    }
}
