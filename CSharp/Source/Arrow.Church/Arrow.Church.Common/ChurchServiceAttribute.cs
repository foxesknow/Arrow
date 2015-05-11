using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common.Data;

namespace Arrow.Church.Common
{
    [AttributeUsage(AttributeTargets.Interface,AllowMultiple=false,Inherited=true)]
	public sealed class ChurchServiceAttribute : Attribute
    {
		public ChurchServiceAttribute(string serviceName, Type messageProtocolType)
		{
			if(serviceName==null) throw new ArgumentNullException("serviceName");
			if(string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentException("serviceName");

			if(messageProtocolType==null) throw new ArgumentNullException("messageProtocolType");
			if(typeof(MessageProtocol).IsAssignableFrom(messageProtocolType)==false) throw new ArgumentException("not a message protocol","messageProtocolType");
			if(messageProtocolType.GetConstructor(Type.EmptyTypes)==null) throw new ArgumentException("message protocol does not have a default constructor","messageProtocolType");

			this.ServiceName=serviceName;
			this.MessageProtocolType=messageProtocolType;
		}

		public Type MessageProtocolType{get;private set;}

		public string ServiceName{get;private set;}
    }
}
