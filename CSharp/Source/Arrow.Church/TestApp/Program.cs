using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common;
using Arrow.Church.Server;

namespace TestApp
{
	class Program
	{
		static void Main(string[] args)
		{
			ServiceContainer host=new ServiceContainer();
			host.Add("Foo",new FooService());
		}
	}

	public class FooService : ChurchServiceBase, IFoo
	{
		public async Task<int> Print()
		{
			return await Task.FromResult(1);
		}

		Task IFoo.Log(object data)
		{
			return Void();
		}

	}


	[ChurchService]
	public interface IFoo
	{
		Task<int> Print();
		Task Log(object data);
	}
}
