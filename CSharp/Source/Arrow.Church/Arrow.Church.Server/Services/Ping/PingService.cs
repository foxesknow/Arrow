using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common.Services.Ping;

namespace Arrow.Church.Server.Services.Ping
{
	public sealed class PingService : ChurchService<IPingService>, IPingService
	{
		Task<PingResponse> IPingService.Ping(PingRequest request)
		{
			if(request==null) throw new ArgumentNullException("request");

			var response=new PingResponse(request.PingID);
			return Task.FromResult(response);
		}
	}
}
