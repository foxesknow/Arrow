using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.DI
{
	/// <summary>
	/// Specifies the lifetime of an object within the container
	/// </summary>
	public enum Lifetime
	{
		/// <summary>
		/// Only one instance is created, and is shared
		/// </summary>
		Singleton,
		
		/// <summary>
		/// A new instance is created each time
		/// </summary>
		Transient
	}
}
