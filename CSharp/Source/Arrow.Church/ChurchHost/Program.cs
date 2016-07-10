using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using Arrow.Application;
using Arrow.Application.Service;

namespace ChurchHost
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			ApplicationRunner.Run(ServiceMain,args);
		}

		static void ServiceMain(string[] args)
		{
			var service=new InteractiveConsoleService<ChurchHostService>();
			service.Run(args);
		}
	}
}
