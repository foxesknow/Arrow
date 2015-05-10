using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Church.Common;
using Arrow.Church.Server;
using Arrow.Church.Client;
using Arrow.Church.Common.Data.DotNet;
using Arrow.Configuration;
using Arrow.Xml.ObjectCreation;
using Arrow.Church.Server.HostBuilder;

namespace TestApp
{
	class Program
	{
		static void Main(string[] args)
		{
			ListenerMain(args);
		}

		static void ListenerMain(string[] args)
		{
			var hostConfigXml=AppConfig.GetSectionXml("App","Hosts/Main");
			var builder=XmlCreation.Create<ServiceHostBuilder>(hostConfigXml);

			try
			{
				using(var host=builder.Build())
				{
					host.Start();
				
					var factory=ProxyManager.FactoryFor<IFoo>();			
					var foo=factory.Create(host.Endpoint,"Foo");

					try
					{
						for(int i=1; i<10; i++)
						{
							var task=foo.Divide(new BinaryOperationRequest(){Lhs=20*i,Rhs=5});
							Console.WriteLine(task.Result);
						}

						foo.DoNothing();
					}
					catch(Exception e)
					{
						Console.Error.WriteLine(e);
					}

					Console.ReadLine();
				}
			}
			catch(Exception e)
			{
				Console.Error.WriteLine(e);
			}
		}
	}

	[ServiceName("Foo")]
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


	[ChurchService(typeof(SerializationMessageProtocol))]
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
