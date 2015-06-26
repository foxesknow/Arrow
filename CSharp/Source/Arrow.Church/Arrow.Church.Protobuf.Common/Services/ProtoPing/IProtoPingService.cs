using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common;

namespace Arrow.Church.Protobuf.Common.Services.ProtoPing
{
	[ChurchService("ProtoPing",typeof(ProtobufMessageProtocol))]
	public interface IProtoPingService
	{
		Task<ProtoPingResponse> Ping(ProtoPingRequest request);
	}
}
