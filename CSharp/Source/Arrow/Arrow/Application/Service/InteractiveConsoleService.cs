using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;

namespace Arrow.Application.Services
{
	/// <summary>
	/// Runs a service as a console application if the app was started as a console process,
	/// or runs the service as a Windows service if it was started from the service control manager
	/// </summary>
	/// <typeparam name="TService">The type of service</typeparam>
	public class InteractiveConsoleService<TService> where TService:InteractiveServiceBase, new()
	{
		/// <summary>
		/// Runs the service as either a console app or Windows service
		/// </summary>
		/// <param name="args">Any command line arguments for the service</param>
		public void Run(string[] args)
		{
			if(Environment.UserInteractive)
			{
				RunAsConsole(args);
			}
			else
			{
				RunAsService(args);
			}
		}

		/// <summary>
		/// Runs the service as a console application.
		/// The service will run until the WaitForShutdown() method returns
		/// </summary>
		/// <param name="args">Any command line argument for the service</param>
		protected virtual void RunAsConsole(string[] args)
		{
			var service=new TService();
			service.DoStart(args);

			try
			{
				WaitForShutdown(service,args);
			}
			finally
			{
				service.DoStop();
			}
		}

		/// <summary>
		/// Wait for some external signal to determine when the service should shutdown.
		/// The default implementation prompts the user to type "exit" in order to shutdown
		/// </summary>
		/// <param name="args">Any command line arguments passed on the command line</param>
		/// <param name="service">The service which is running</param>
		protected virtual void WaitForShutdown(TService service, string[] args)
		{
			bool keepRunning=true;

			while(keepRunning)
			{
				Console.Write("Type exit to exit: ");
				string line=Console.ReadLine().Trim();

				if(line=="exit") 
				{
					keepRunning=false;
				}
				else if(line!="")
				{
					ProcessUserInput(service,line);
				}
			}
		}

		/// <summary>
		/// Processes the line of text entered by the user.
		/// The base implementation does nothing with the line
		/// </summary>
		/// <param name="line">The line of text entered by the user</param>
		/// <param name="service">The service which is running</param>
		protected virtual void ProcessUserInput(TService service, string line)
		{
			// Does nothing
		}

		/// <summary>
		/// Runs the service as a Windows service
		/// </summary>
		/// <param name="args">Any command line arguments passed on the service command line</param>
		protected virtual void RunAsService(string[] args)
		{
			ServiceBase[] servicesToRun={new TService()};
			ServiceBase.Run(servicesToRun);
		}
	}
}
