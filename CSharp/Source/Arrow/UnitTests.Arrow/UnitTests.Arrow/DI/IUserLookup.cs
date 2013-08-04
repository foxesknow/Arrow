using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.DI;

namespace UnitTests.Arrow.DI
{
	public interface IUserLookup
	{
		bool DoesUseExist(string username);
	}

	class UserLookupStub : IUserLookup
	{
		public bool DoesUseExist(string username)
		{
			return false;
		}
	}

	class ConstructorTest
	{
		public ConstructorTest(IContainer container, IUserLookup userLookup)
		{
			this.Container=container;
			this.UserLookup=userLookup;
		}

		public IContainer Container{get;private set;}

		public IUserLookup UserLookup{get;private set;}
	}

	interface IFoo
	{
		void HandleFoo();
	}

	interface IBar
	{
		void HandleBar();
	}

	class FooBar : IFoo, IBar
	{

		#region IFoo Members

		public void HandleFoo()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IBar Members

		void IBar.HandleBar()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
