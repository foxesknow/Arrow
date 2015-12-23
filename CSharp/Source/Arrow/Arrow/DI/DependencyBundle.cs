using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.DI
{
	/// <summary>
	/// Allows a group of related dependencies to be registered
	/// </summary>
	public abstract class DependencyBundle
	{
		/// <summary>
		/// Registers types into a dependency container
		/// </summary>
		/// <param name="container">The container to register types into</param>
		/// <returns>The container that should be used for subsequent registrations</returns>
		public IDIContainerRegister RegisterBundle(IDIContainerRegister container)
		{
			var newContainer=Register(container);
			if(newContainer==null) throw new ContainerException("DependencyBundle.Register did not return a container");

			return newContainer;
		}

		/// <summary>
		/// Registers types into a dependency container.
		/// Register returns a container to allow the method to create a NewScope if required.
		/// If this isn't required then just return the supplied container
		/// </summary>
		/// <param name="container">The container to register types into</param>
		/// <returns>The container that should be used for subsequent registrations</returns>
		protected abstract IDIContainerRegister Register(IDIContainerRegister container);
	}
}
