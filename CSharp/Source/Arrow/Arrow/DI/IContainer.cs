using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.DI
{
	/// <summary>
	/// Defines the standard behaviour for a container
	/// </summary>
	public interface IContainer
	{
		/// <summary>
		/// Resolves a type to an underlying implementation
		/// </summary>
		/// <param name="type">The type to resolve</param>
		/// <returns>An instance of the type</returns>
		object Resolve(Type type);
	}	
}
