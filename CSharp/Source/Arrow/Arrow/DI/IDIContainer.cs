using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.DI
{
	/// <summary>
	/// Defines the standard behaviour for a container
	/// </summary>
	public interface IDIContainer
	{
		/// <summary>
		/// Resolves a type to an underlying implementation
		/// </summary>
		/// <param name="type">The type to resolve</param>
		/// <returns>An instance of the type</returns>
		object Resolve(Type type);

		/// <summary>
		/// Register an existing instance as a singleton
		/// </summary>
		/// <typeparam name="T">The item to register</typeparam>
		/// <param name="exposedType">The types to expose from the item</param>
		/// <param name="item">The item to register</param>
		IDIContainer RegisterInstance<T>(IList<Type> exposedType, T item) where T:class;

		/// <summary>
		/// Registers a concrete type against multiple exposed types
		/// </summary>
		/// <param name="exposedTypes">The types to expose</param>
		/// <param name="contreteType">A concreate implementation of the exposed type</param>
		/// <param name="lifetime">The lifetime of the type</param>
		IDIContainer Register(IList<Type> exposedTypes, Type contreteType, Lifetime lifetime);

		/// <summary>
		/// Creates a new container scope.
		/// When resolving against the new scope the container will defer to its parent
		/// if it cannot resolve a type
		/// </summary>
		/// <returns>A new container scope</returns>
		IDIContainer NewScope();
	}	
}
