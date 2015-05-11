using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Arrow.Church.Common;
using Arrow.Church.Common.Data;

namespace Arrow.Church.Server
{
    public abstract class ChurchService
    {
		private readonly Type m_ServiceInterface;
		private readonly MessageProtocol m_MessageProtocol;

		private CancellationTokenSource m_StopCancellationTokenSource=new CancellationTokenSource();
		private ManualResetEvent m_StopEvent=new ManualResetEvent(false);

		internal ChurchService(Type serviceInterface)
		{
			if(serviceInterface==null) throw new ArgumentNullException("serviceInterface");

			m_ServiceInterface=serviceInterface;
			m_MessageProtocol=ExtractMessageProtocol(serviceInterface);
		}

		internal void ContainerStart()
		{
			if(m_StopCancellationTokenSource!=null) m_StopCancellationTokenSource.Dispose();
			m_StopCancellationTokenSource=new CancellationTokenSource();

			m_StopEvent.Reset();
		}

		internal void ContainerStop()
		{
			m_StopCancellationTokenSource.Cancel();
			m_StopEvent.Set();
		}

		/// <summary>
		/// Signalled when the service should stop
		/// </summary>
		protected EventWaitHandle StopEvent
		{
			get{return m_StopEvent;}
		}

		/// <summary>
		/// Set when the service should stop
		/// </summary>
		protected CancellationToken StopCancellationToken
		{
			get{return m_StopCancellationTokenSource.Token;}
		}

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

		public MessageProtocol MessageProtocol
		{
			get{return m_MessageProtocol;}
		}

		public Type ServiceInterface
		{
			get{return m_ServiceInterface;}
		}

		private static MessageProtocol ExtractMessageProtocol(Type type)
		{
			var attributes=type.GetCustomAttributes(typeof(ChurchServiceAttribute),true);
			if(attributes==null || attributes.Length==0) throw new InvalidOperationException("could not find ChurchService attribute");

			var churchService=(ChurchServiceAttribute)attributes[0];
			return (MessageProtocol)Activator.CreateInstance(churchService.MessageProtocolType);
		}
    }
}
