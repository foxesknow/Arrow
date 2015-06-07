using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Arrow.Church.Common.Services.Ping;

namespace Arrow.Church.Server.Services.Ping
{
	public sealed class PingService : ChurchService<IPingService>, IPingService
	{
		private long m_ServerPingID;

		Task<PingResponse> IPingService.Ping(PingRequest request)
		{
			if(request==null) throw new ArgumentNullException("request");

			long serverPingID=Interlocked.Increment(ref m_ServerPingID);

			var response=new PingResponse(request.PingID,serverPingID);
			return Task.FromResult(response);
		}
	}
}
