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

namespace TestApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Uri n=new Uri("foo://server:80/hello");

			Type t=ProxyBase.GenerateProxy(typeof(IFoo));			
			var ctorArgs=new object[]{null};
			var foo=(IFoo)Activator.CreateInstance(t,ctorArgs);

			foo.Add(new BinaryOperationRequest(){Lhs=20,Rhs=0});
			foo.DoNothing();
		}

		static void ListenerMain(string[] args)
		{
			try
			{
				var protocol=new SerializationMessageProtocol();
				var listener=new TestServiceListener(protocol);
				var host=new ServiceHost(listener,protocol);
				host.ServiceContainer.Add("Foo",new FooService());

				var task=listener.Call("Foo","Divide",new BinaryOperationRequest(){Lhs=20,Rhs=0});

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

	public class FooService : ChurchServiceBase, IFoo
	{
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
			return Void();
		}
	}


	[ChurchService]
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
