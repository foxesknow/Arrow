﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common.Data;

namespace Arrow.Church.Common.Net
{
	public class ServiceCallRequest : IEncodeData
	{
		public ServiceCallRequest(DataDecoder encoder)
		{
			int version=encoder.ReadInt32();
			this.ServiceName=encoder.ReadString();
			this.ServiceMethod=encoder.ReadString();
		}
		
		public ServiceCallRequest(string serviceName, string serviceMethod)
		{
			this.ServiceName=serviceName;
			this.ServiceMethod=serviceMethod;
		}

		public string ServiceName{get;private set;}
		public string ServiceMethod{get;private set;}

		void IEncodeData.Encode(DataEncoder encoder)
		{
			encoder.Write(1); // Version
			encoder.Write(this.ServiceName);
			encoder.Write(this.ServiceMethod);
		}

		public override string ToString()
		{
			return string.Format("Service={0}, Method={1}",this.ServiceName,this.ServiceMethod);
		}
	}
}
