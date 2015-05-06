using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common;

namespace Arrow.Church.Client.ServiceDispatchers
{
	public static class ServiceDispatcherManager
	{
		private static readonly object s_SyncRoot=new object();
		private static readonly Dictionary<Uri,ServiceDispatcher> s_Dispatchers=new Dictionary<Uri,ServiceDispatcher>();

		public static ServiceDispatcher GetServiceDispatcher(Uri endpoint)
		{
			ServiceDispatcher dispatcher=null;

			lock(s_SyncRoot)
			{
				if(s_Dispatchers.TryGetValue(endpoint,out dispatcher)==false)
				{
					var creator=ServiceDispatcherFactory.TryCreate(endpoint.Scheme);
					if(creator==null) throw new ChurchException("scheme not registered: "+endpoint.Scheme);
					
					dispatcher=creator.Create(endpoint);
					s_Dispatchers.Add(endpoint,dispatcher);
				}
			}

			return dispatcher;
		}
	}
}
