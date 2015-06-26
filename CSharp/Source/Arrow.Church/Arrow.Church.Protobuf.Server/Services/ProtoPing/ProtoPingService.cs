using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Arrow.Church.Protobuf.Common.Services.ProtoPing;
using Arrow.Church.Server;

namespace Arrow.Church.Protobuf.Server.Services.ProtoPing
{
	public class ProtoPingService : ChurchService<IProtoPingService>, IProtoPingService
	{
		private long m_ServerPingID;

		Task<ProtoPingResponse> IProtoPingService.Ping(ProtoPingRequest request)
		{
			if(request==null) throw new ArgumentNullException("request");

			long serverPingID=Interlocked.Increment(ref m_ServerPingID);

			var response=new ProtoPingResponse()
			{
				ClientPingID=request.PingID,
				ServerPingID=serverPingID
			};

			return Task.FromResult(response);
		}
	}
}
