using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Arrow.DI
{
	/// <summary>
	/// Provides useful registration methods
	/// </summary>
	public static partial class IDIContainerRegisterExtensions
	{
		/// <summary>
		/// Registers an existing instance
		/// </summary>
		/// <typeparam name="T">The type of the instance</typeparam>
		/// <param name="container">The container to register against</param>
		/// <param name="item">The instance</param>
		/// <returns>The container to use in subsequent calls</returns>
		public static IDIContainerRegister RegisterInstance<T>(this IDIContainerRegister container, T item) where T:class
		{
			if(container==null) throw new ArgumentNullException("container");
			if(item==null) throw new ArgumentNullException("item");

			Type[] exposedTypes={typeof(T)};
			container.RegisterInstance(exposedTypes,item);

			return container;
		}

		/// <summary>
		/// Registers a concrete type against a base type.
		/// The lifetime is set to Transient
		/// </summary>
		/// <typeparam name="B">The base type to expose</typeparam>
		/// <typeparam name="T">The concrete implementation of the base type</typeparam>
		/// <param name="container">The container to register against</param>
		/// <returns>The container to use in subsequent calls</returns>
		public static IDIContainerRegister Register<B,T>(this IDIContainerRegister container) where T:class,B
		{
			if(container==null) throw new ArgumentNullException("container");

			Type[] exposedTypes={typeof(B)};
			container.Register(exposedTypes,typeof(T),Lifetime.Transient);

			return container;
		}

		/// <summary>
		/// Registers a concrete type against a base type
		/// </summary>
		/// <typeparam name="B">The base type to expose</typeparam>
		/// <typeparam name="T">The concrete implementation of the base type</typeparam>
		/// <param name="container">The container to register against</param>
		/// <param name="lifetime">The lifetime of the T</param>
		/// <returns>The container to use in subsequent calls</returns>
		public static IDIContainerRegister Register<B,T>(this IDIContainerRegister container, Lifetime lifetime) where T:class,B
		{
			if(container==null) throw new ArgumentNullException("container");

			Type[] exposedTypes={typeof(B)};
			container.Register(exposedTypes,typeof(T),lifetime);

			return container;
		}

		/// <summary>
		/// Registers a concrete type against an exposed type
		/// </summary>
		/// <param name="container">The container to register against</param>
		/// <param name="exposedType">The type to expose</param>
		/// <param name="contreteType">A concreate implementation of the exposed type</param>
		/// <param name="lifetime">The lifetime of the type</param>
		/// <returns>The container to use in subsequent calls</returns>
		public static IDIContainerRegister Register(this IDIContainerRegister container, Type exposedType, Type contreteType, Lifetime lifetime)
		{
			if(container==null) throw new ArgumentNullException("container");
			if(exposedType==null) throw new ArgumentNullException("exposedType");
			if(contreteType==null) throw new ArgumentNullException("contreteType");

			Type[] exposedTypes={exposedType};
			container.Register(exposedTypes,contreteType,lifetime);

			return container;
		}

		/// <summary>
		/// Registers a bundle into the container
		/// </summary>
		/// <typeparam name="TBundle">The bundle to register</typeparam>
		/// <param name="container">The container to register into</param>
		/// <returns>The container to use in subsequent calls</returns>
		public static IDIContainerRegister RegisterBundle<TBundle>(this IDIContainerRegister container) where TBundle : DependencyBundle, new()
		{
			if(container==null) throw new ArgumentNullException("container");

			var bundle=new TBundle();
			return bundle.RegisterBundle(container);
		}

		/// <summary>
		/// Registers a bundle into the container
		/// </summary>
		/// <param name="container">The container to register into</param>
		/// <param name="bundle">The bundle to register</param>
		/// <returns>The container to use in subsequent calls</returns>
		public static IDIContainerRegister RegisterBundle(this IDIContainerRegister container, DependencyBundle bundle)
		{
			if(container==null) throw new ArgumentNullException("container");
			if(bundle==null) throw new ArgumentNullException("bundle");

			return bundle.RegisterBundle(container);
		}

		/// <summary>
		/// Registers all public bundles in an assembly
		/// </summary>
		/// <param name="container">The container to register into</param>
		/// <param name="assembly">The assembly to scan for bundles</param>
		/// <returns>The container to use in subsequent calls</returns>
		public static IDIContainerRegister RegisterBundlesInAssemby(this IDIContainerRegister container, Assembly assembly)
		{
			if(container==null) throw new ArgumentNullException("container");
			if(assembly==null) throw new ArgumentNullException("assembly");

			foreach(Type type in assembly.GetTypes())
			{
				if(type.IsClass && type.IsAbstract==false && type.IsPublic && typeof(DependencyBundle).IsAssignableFrom(type))
				{
					// We'll need a default constructor
					var ctor=type.GetConstructor(Type.EmptyTypes);
					if(ctor!=null)
					{
						var bundle=(DependencyBundle)ctor.Invoke(null);
						container=bundle.RegisterBundle(container);
					}
				}
			}

			return container;

		}
	}
}
