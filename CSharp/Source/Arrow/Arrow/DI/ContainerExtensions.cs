using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.DI
{
	/// <summary>
	/// Provides useful resolution methods
	/// </summary>
	public static class ContainerExtensions
	{
		/// <summary>
		/// Resolves a type to an underlying implementation
		/// </summary>
		/// <typeparam name="T">The type to resolve</typeparam>
		/// <param name="container">The container to resolve against</param>
		/// <returns>An instance of T</returns>
		public static T Resolve<T>(this IContainer container)
		{
			if(container==null) throw new ArgumentNullException("container");

			return (T)container.Resolve(typeof(T));
		}
	}
}
