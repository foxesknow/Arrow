using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Arrow.Church.Common;
using Arrow.Church.Server;
using Arrow.Church.Client;
using Arrow.Church.Common.Data.DotNet;
using Arrow.Configuration;
using Arrow.Xml.ObjectCreation;
using Arrow.Church.Server.HostBuilder;
using Arrow.Church.Common.Services.Ping;
using Arrow.Church.Common.Services.ServiceRegistrar;
using Arrow.Church.Common.Services.VirtualDirectory;
using Arrow.Church.Protobuf.Common.Services.ProtoPing;

namespace TestApp
{
	class Program
	{
		static void Main(string[] args)
		{
			//var task=Run(args);
            var task=ResolveViaRegistrar();
			task.Wait();
		}

        static async Task ResolveViaRegistrar()
        {
            var registrarEndpoint=new Uri("net://localhost:999/");
            var proxyFactory=new RegistrarProxyFactory(WellKnownService.ServiceRegistrar,registrarEndpoint);

            var pingService=await proxyFactory.GetServiceAsync<IPingService>(WellKnownService.Ping);
            var pingResponse=await pingService.Ping(new PingRequest());
            Console.WriteLine(pingResponse.ServerLocal);

            var protoPingService=await proxyFactory.GetServiceAsync<IProtoPingService>("ProtoPing");
            
            for(int i=0; i<10; i++)
            {
                var protoPingResponse=await protoPingService.Ping(new ProtoPingRequest());
                Console.WriteLine("ClientPingID={0}, ServerPingID={1}",protoPingResponse.ClientPingID,protoPingResponse.ServerPingID);
            }
        }

		static async Task Run(string[] args)
		{
			var endpoint=new Uri("net://localhost:899");

			
			var fooFactory=ProxyManager.FactoryFor<IFoo>();			
			var foo=fooFactory.Create("Foo",endpoint);

			try
			{

				var pingFactory=ProxyManager.FactoryFor<IProtoPingService>();
				var ping=pingFactory.Create("ProtoPing",endpoint);

				var response=await ping.Ping(new ProtoPingRequest());
				Console.WriteLine(response);

				var dirFactory=ProxyManager.FactoryFor<IVirtualDirectoryService>();
				var dir=dirFactory.Create("Dir",endpoint);

				var download=await dir.Download(new PathRequest("AdventureWorks2012_Data.mdf"));
				Console.WriteLine("got {0} bytes",download.Data.Length);
			}
			catch(Exception e)
			{
				Console.Error.WriteLine(e);
			}
		}
	}

	
	public class FooService : ChurchService<IFoo>, IFoo
	{
		public FooService() : base()
		{
			this.NothingText="Does nothing!";
		}

		public string NothingText{get;set;}

		public Task<BinaryOperationResponse> Add(BinaryOperationRequest request)
		{
			var answer=new BinaryOperationResponse
			{
				Answer=request.Lhs+request.Rhs
			};

			return Task.FromResult(answer);
		}

		public async Task<BinaryOperationResponse> Divide(BinaryOperationRequest request)
		{
			return await Task.Run(()=>
			{
				return new BinaryOperationResponse
				{
					Answer=request.Lhs/request.Rhs
				};
			});
		}

		public Task DoNothing()
		{
			Console.WriteLine(this.NothingText);
			return Void();
		}
	}


	[ChurchService("Foo",typeof(SerializationMessageProtocol))]
	public interface IFoo
	{
		Task<BinaryOperationResponse> Add(BinaryOperationRequest request);
		Task<BinaryOperationResponse> Divide(BinaryOperationRequest request);
		Task DoNothing();
	}

	[Serializable]
	public class BinaryOperationRequest
	{
		public int Lhs{get;set;}
		public int Rhs{get;set;}
	}

	[Serializable]
	public class BinaryOperationResponse
	{
		public int Answer{get;set;}

		public override string ToString()
		{
			return this.Answer.ToString();
		}
	}
}
