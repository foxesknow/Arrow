using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.DI
{
	/// <summary>
	/// Allows for registration in a DI container
	/// </summary>
	public interface IDIContainerRegister : IDIContainer
	{
		/// <summary>
		/// Register an existing instance as a singleton
		/// </summary>
		/// <typeparam name="T">The item to register</typeparam>
		/// <param name="exposedType">The types to expose from the item</param>
		/// <param name="item">The item to register</param>
		/// <returns>The DI container</returns>
		IDIContainerRegister RegisterInstance<T>(IList<Type> exposedType, T item) where T:class;

		/// <summary>
		/// Registers a concrete type against multiple exposed types
		/// </summary>
		/// <param name="exposedTypes">The types to expose</param>
		/// <param name="contreteType">A concreate implementation of the exposed type</param>
		/// <param name="lifetime">The lifetime of the type</param>
		/// <returns>The DI container</returns>
		IDIContainerRegister Register(IList<Type> exposedTypes, Type contreteType, Lifetime lifetime);

		/// <summary>
		/// Registers a factory method that will be called to create an instance
		/// </summary>
		/// <typeparam name="T">The type to register</typeparam>
		/// <param name="lifetime">The lifetime of the type</param>
		/// <param name="factory">The factory method that creates they type</param>
		/// <returns>The DI container</returns>
		IDIContainerRegister RegisterFactory<T>(Lifetime lifetime, Func<IDIContainer,T> factory) where T:class;

		/// <summary>
		/// Creates a new container scope.
		/// When resolving against the new scope the container will defer to its parent
		/// if it cannot resolve a type
		/// </summary>
		/// <returns>A new container scope</returns>
		IDIContainerRegister NewScope();

	}
}
