using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading
{
	public partial class TaskWorkQueue
	{
		interface IWork
		{
			void Execute();
			void Cancel();
		}

		class Work : IWork
		{
			private readonly TaskCompletionSource<bool> m_TaskCompletionSource=new TaskCompletionSource<bool>();
			private readonly Action m_Action;

			public Work(Action action)
			{
				m_Action=action;
			}

			public Task Task
			{
				get{return m_TaskCompletionSource.Task;}
			}

			public void Execute()
			{
				try
				{
					m_Action();
					m_TaskCompletionSource.SetResult(true);
				}
				catch(Exception e)
				{
					m_TaskCompletionSource.SetException(e);
				}
			}

			public void Cancel()
			{
				m_TaskCompletionSource.SetCanceled();
			}
		}

		class Work<T> : IWork
		{
			private readonly TaskCompletionSource<T> m_TaskCompletionSource=new TaskCompletionSource<T>();
			private readonly Func<T> m_Function;

			public Work(Func<T> function)
			{
				m_Function=function;
			}

			public Task<T> Task
			{
				get{return m_TaskCompletionSource.Task;}
			}

			public void Execute()
			{
				try
				{
					m_TaskCompletionSource.SetResult(m_Function());
				}
				catch(Exception e)
				{
					m_TaskCompletionSource.SetException(e);
				}
			}

			public void Cancel()
			{
				m_TaskCompletionSource.SetCanceled();
			}
		}
	}
}
