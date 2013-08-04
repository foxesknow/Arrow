using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.DI
{
	/// <summary>
	/// Provides useful registration methods
	/// </summary>
	public static partial class ContainerRegistrationExtensions
	{
		/// <summary>
		/// Registers an existing instance
		/// </summary>
		/// <typeparam name="T">The type of the instance</typeparam>
		/// <param name="container">The container to register against</param>
		/// <param name="item">The instance</param>
		public static void RegisterInstance<T>(this IContainerRegistration container, T item) where T:class
		{
			if(container==null) throw new ArgumentNullException("container");

			Type[] exposedTypes={typeof(T)};
			container.RegisterInstance(exposedTypes,item);
		}

		/// <summary>
		/// Registers a concrete type against a base type.
		/// The lifetime is set to Transient
		/// </summary>
		/// <typeparam name="B">The base type to expose</typeparam>
		/// <typeparam name="T">The concrete implementation of the base type</typeparam>
		/// <param name="container">The container to register against</param>
		public static void Register<B,T>(this IContainerRegistration container) where T:class,B
		{
			if(container==null) throw new ArgumentNullException("container");

			Type[] exposedTypes={typeof(B)};
			container.Register(exposedTypes,typeof(T),Lifetime.Transient);
		}

		/// <summary>
		/// Registers a concrete type against a base type
		/// </summary>
		/// <typeparam name="B">The base type to expose</typeparam>
		/// <typeparam name="T">The concrete implementation of the base type</typeparam>
		/// <param name="container">The container to register against</param>
		/// <param name="lifetime">The lifetime of the T</param>
		public static void Register<B,T>(this IContainerRegistration container, Lifetime lifetime) where T:class,B
		{
			if(container==null) throw new ArgumentNullException("container");

			Type[] exposedTypes={typeof(B)};
			container.Register(exposedTypes,typeof(T),lifetime);
		}

		/// <summary>
		/// Registers a concrete type against an exposed type
		/// </summary>
		/// <param name="container">The container to register against</param>
		/// <param name="exposedType">The type to expose</param>
		/// <param name="contreteType">A concreate implementation of the exposed type</param>
		/// <param name="lifetime">The lifetime of the type</param>
		public static void Register(this IContainerRegistration container, Type exposedType, Type contreteType, Lifetime lifetime)
		{
			if(container==null) throw new ArgumentNullException("container");

			Type[] exposedTypes={exposedType};
			container.Register(exposedTypes,contreteType,Lifetime.Transient);
		}
	}
}
