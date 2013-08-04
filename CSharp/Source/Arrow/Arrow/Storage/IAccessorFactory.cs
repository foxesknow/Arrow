using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Storage
{
	/// <summary>
	/// Defines a factory for a particular Accessor instance
	/// </summary>
	public interface IAccessorFactory
	{
		/// <summary>
		/// Creates an Accessor for the specified Uri
		/// </summary>
		/// <param name="uri">The uri to access</param>
		/// <returns>An accessor</returns>
		Accessor Create(Uri uri);
	}
}
