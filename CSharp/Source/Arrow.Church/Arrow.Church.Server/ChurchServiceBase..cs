using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Church.Common;
using Arrow.Church.Common.Data;

namespace Arrow.Church.Server
{
	public abstract class ChurchService<T> : ChurchService where T:class
	{
		protected ChurchService() : base(ExtractMessageProtocol(typeof(T)))
		{
		}

		private static MessageProtocol ExtractMessageProtocol(Type type)
		{
			var attributes=type.GetCustomAttributes(typeof(ChurchServiceAttribute),true);
			if(attributes==null || attributes.Length==0) throw new InvalidOperationException("could not find ChurchService attribute");

			var churchService=(ChurchServiceAttribute)attributes[0];
			return (MessageProtocol)Activator.CreateInstance(churchService.MessageProtocolType);
		}
	}
}
