using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common;
using Arrow.Church.Common.Data;

namespace Arrow.Church.Server
{
    public abstract class ChurchService
    {
		private readonly MessageProtocol m_MessageProtocol;

		internal ChurchService(MessageProtocol messageProtocol)
		{
			if(messageProtocol==null) throw new ArgumentNullException("messageProtocol");

			m_MessageProtocol=messageProtocol;
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
    }
}
