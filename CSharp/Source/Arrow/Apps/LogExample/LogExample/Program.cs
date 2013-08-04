using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Logging;
using Arrow.Application;

namespace LogExample
{
	class Program
	{
		static void Main(string[] args)
		{
			ApplicationRunner.Run(()=>Run(args));
		}

		static void Run(string[] args)
		{
			var log=LogManager.GetDefaultLog();
			log.Info("info");
		}
	}
}
