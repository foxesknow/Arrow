using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Client.Proxy;
using Arrow.Church.Common.Data;

namespace Arrow.Church.Client
{
	partial class ServiceDispatcher
	{
		interface IOutstandingCall
		{
			void Accept(bool isFaulted, object response);
			MessageProtocol MessageProtocol{get;}
		}

		class OutstandingCall : IOutstandingCall
		{
			private readonly TaskCompletionSource<bool> m_Source=new TaskCompletionSource<bool>();

			public OutstandingCall(ProxyBase proxy)
			{
				this.MessageProtocol=proxy.MessageProtocol;
			}

			public void Accept(bool isFaulted, object response)
			{
				if(isFaulted)
				{
					m_Source.SetException((Exception)response);
				}
				else
				{
					m_Source.SetResult(true);
				}
			}

			public MessageProtocol MessageProtocol{get;private set;}

			public Task GetTask()
			{
				return m_Source.Task;
			}
		}

		class OutstandingCall<T> : IOutstandingCall
		{
			private readonly TaskCompletionSource<T> m_Source=new TaskCompletionSource<T>();

			public OutstandingCall(ProxyBase proxy)
			{
				this.MessageProtocol=proxy.MessageProtocol;
			}

			public void Accept(bool isFaulted, object response)
			{
				if(isFaulted)
				{
					m_Source.SetException((Exception)response);
				}
				else
				{
					m_Source.SetResult((T)response);
				}
			}

			public MessageProtocol MessageProtocol{get;private set;}

			public Task<T> GetTask()
			{
				return m_Source.Task;
			}
		}
	}
}
