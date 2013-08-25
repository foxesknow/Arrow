using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Collections.Caching
{
	/// <summary>
	/// The purge mode for a cache
	/// </summary>
	public enum PurgeMode
	{
		/// <summary>
		/// The cache will purge items when purge point methods are called
		/// </summary>
		Managed,

		/// <summary>
		/// The owner must request the cache purge any items by calling Purge()
		/// </summary>
		Unmanaged
	}
}
