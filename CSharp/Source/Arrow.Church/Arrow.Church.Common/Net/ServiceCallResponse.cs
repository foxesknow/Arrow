using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common.Data;

namespace Arrow.Church.Common.Net
{
	public class ServiceCallResponse : IEncodeData
	{
		public ServiceCallResponse(DataDecoder encoder)
		{
			int version=encoder.ReadInt32();
			this.ServiceName=encoder.ReadString();
			this.ServiceMethod=encoder.ReadString();
			this.IsFaulted=encoder.ReadBoolean();
		}
		
		public ServiceCallResponse(string serviceName, string serviceMethod, bool isFaulted)
		{
			this.ServiceName=serviceName;
			this.ServiceMethod=serviceMethod;
			this.IsFaulted=isFaulted;
		}

		public string ServiceName{get;private set;}
		public string ServiceMethod{get;private set;}
		public bool IsFaulted{get;private set;}

		void IEncodeData.Encode(DataEncoder encoder)
		{
			encoder.Write(1); // Version
			encoder.Write(this.ServiceName);
			encoder.Write(this.ServiceMethod);
			encoder.Write(this.IsFaulted);
		}
	}
}
