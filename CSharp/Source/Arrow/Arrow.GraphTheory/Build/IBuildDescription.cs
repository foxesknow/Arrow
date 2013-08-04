using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.GraphTheory.Build
{
	/// <summary>
	/// Describes build information to the build generator
	/// </summary>
	/// <typeparam name="T">The build item type</typeparam>
	public interface IBuildDescription<T>
	{
		/// <summary>
		/// Returns the target of the build
		/// </summary>
		T Target{get;}
		
		/// <summary>
		/// Returns the items the target depends upon for its generation
		/// </summary>
		ICollection<T> Dependencies{get;}
	}
}
