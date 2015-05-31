using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common.Data.DotNet;

namespace Arrow.Church.Common.Services.Ping
{
	[ChurchService("Ping",typeof(SerializationMessageProtocol))]
	public interface IPingService
	{
		Task<PingResponse> Ping(PingRequest request);
	}
}
