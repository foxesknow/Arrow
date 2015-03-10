using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Arrow.Net
{
	public abstract class SocketProcessor
	{
		public event EventHandler<EventArgs> Disconnected;
		public event EventHandler<EventArgs> NetworkFault;

		private long m_KeepReading=1;

		public bool KeepReading
		{
			get{return Interlocked.Read(ref m_KeepReading)==1;}
			set
			{
				Interlocked.Exchange(ref m_KeepReading,(value ? 1 : 0));
			}
		}

		protected bool CanProcessBytesRead(int bytesRead)
		{
			if(bytesRead==0)
			{
				OnDisconnected(EventArgs.Empty);
				return false;
			}
			else
			{
				return true;
			}
		}


		protected void OnDisconnected(EventArgs args)
		{
			var handler=this.Disconnected;
			if(handler!=null) handler(this,args);
		}

		protected void OnNetworkFault(EventArgs args)
		{
			var handler=this.NetworkFault;
			if(handler!=null) handler(this,args);
		}
	}
}
