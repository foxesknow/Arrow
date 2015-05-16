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
    public abstract partial class ChurchService
    {
		private readonly Type m_ServiceInterface;
		private readonly MessageProtocol m_MessageProtocol;

		private IHost m_Host=NotRunningHost.Instance;

		internal ChurchService(Type serviceInterface)
		{
			if(serviceInterface==null) throw new ArgumentNullException("serviceInterface");

			m_ServiceInterface=serviceInterface;
			m_MessageProtocol=ExtractMessageProtocol(serviceInterface);
		}

		internal void ContainerStart(IHost host)
		{
			m_Host=host;
		}

		internal void ContainerStop()
		{
			m_Host=NotRunningHost.Instance;
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

		/// <summary>
		/// Returns access to host features.
		/// These are only available once the host has been started
		/// </summary>
		protected IHost Host
		{
			get{return m_Host;}
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

		internal void ContainerDispose()
		{
		}
	}
}
