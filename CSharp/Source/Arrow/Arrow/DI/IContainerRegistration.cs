using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.DI
{
	/// <summary>
	/// Defines the behaviour for a DI container that allows types to be registered
	/// </summary>
	public interface IContainerRegistration
	{
		/// <summary>
		/// Register an existing instance as a singleton
		/// </summary>
		/// <typeparam name="T">The item to register</typeparam>
		/// <param name="exposedType">The types to expose from the item</param>
		/// <param name="item">The item to register</param>
		void RegisterInstance<T>(IList<Type> exposedType, T item) where T:class;

		/// <summary>
		/// Registers a concrete type against multiple exposed types
		/// </summary>
		/// <param name="exposedTypes">The types to expose</param>
		/// <param name="contreteType">A concreate implementation of the exposed type</param>
		/// <param name="lifetime">The lifetime of the type</param>
		void Register(IList<Type> exposedTypes, Type contreteType, Lifetime lifetime);
	}
}
