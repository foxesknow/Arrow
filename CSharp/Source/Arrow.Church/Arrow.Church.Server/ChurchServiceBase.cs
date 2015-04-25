using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Server
{
    public abstract class ChurchServiceBase
    {
		/// <summary>
		/// Create a void task
		/// </summary>
		/// <returns>A task which has no return value</returns>
		protected Task Void()
		{
			var source=new TaskCompletionSource<bool>();
			source.SetResult(true);

			return source.Task;
		}
    }
}
