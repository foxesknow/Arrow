using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Church.Common;
using Arrow.Church.Server;
using Arrow.Church.Server.ServiceListeners;
using Arrow.Church.Common.Data.DotNet;
using Arrow.Church.Client.Proxy;
using Arrow.Church.Client.ServiceDispatchers;

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
			try
			{
				Uri endpoint=new Uri("net://localhost:8999");
				
				using(var host=new ServiceHost(endpoint))
				{
					host.ServiceContainer.Add(new FooService());
					host.Start();
				
					var factory=ProxyBase.GetProxyFactory(typeof(IFoo));			
					var foo=factory.Create<IFoo>(endpoint,"Foo");

					try
					{
						for(int i=1; i<10; i++)
						{
							var task=foo.Divide(new BinaryOperationRequest(){Lhs=20*i,Rhs=5});
							Console.WriteLine(task.Result);
						}
					}
					catch
					{
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
		}

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
			Console.WriteLine("doing nothing!");
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
