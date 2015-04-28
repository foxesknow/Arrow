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

		static void ProxyMain(string[] args)
		{
			Uri n=new Uri("foo://server:80");

			var factory=ProxyBase.GetProxyFactory(typeof(IFoo));			
			var foo=(IFoo)factory(null,"hello");

			foo.Add(new BinaryOperationRequest(){Lhs=20,Rhs=0});
			foo.DoNothing();
		}

		static void ListenerMain(string[] args)
		{
			try
			{
				Uri endpoint=new Uri("church-mem://calc");
				
				var listener=new InProcessServiceListener(endpoint);
				var host=new ServiceHost(listener);
				host.ServiceContainer.Add("Foo",new FooService());
				
				var dispatcher=new InProcessServiceDispatcher(endpoint);
				var factory=ProxyBase.GetProxyFactory(typeof(IFoo));			
				var foo=(IFoo)factory(dispatcher,"Foo");

				var task=foo.Divide(new BinaryOperationRequest(){Lhs=20,Rhs=10});
				//var task=foo.DoNothing();

				task.Wait();
				Console.WriteLine(task.Result);
				Console.ReadLine();
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
