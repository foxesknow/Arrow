using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Server.HostBuilder
{
	public sealed class ServiceDetails
	{
		/// <summary>
		/// The optional name for the service
		/// </summary>
		public string Name{get;set;}
		
		/// <summary>
		/// The service
		/// </summary>
		public ChurchService Service{get;set;}
	}
}
