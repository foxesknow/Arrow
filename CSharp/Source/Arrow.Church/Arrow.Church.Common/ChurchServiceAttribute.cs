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
		public ChurchServiceAttribute(Type messageProtocolType)
		{
			if(messageProtocolType==null) throw new ArgumentNullException("messageProtocolType");
			if(typeof(MessageProtocol).IsAssignableFrom(messageProtocolType)==false) throw new ArgumentException("not a message protocol","messageProtocolType");
			if(messageProtocolType.GetConstructor(Type.EmptyTypes)==null) throw new ArgumentException("message protocol does not have a default constructor","messageProtocolType");

			this.MessageProtocolType=messageProtocolType;
		}

		public Type MessageProtocolType{get;private set;}
    }
}
