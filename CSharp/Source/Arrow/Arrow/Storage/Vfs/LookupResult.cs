using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Storage.Vfs
{
	/// <summary>
	/// The outcome of looking up something in a directory node
	/// </summary>
	public enum LookupResult
	{
		/// <summary>
		/// The item was found
		/// </summary>
		Success,
		
		/// <summary>
		/// The item was not found
		/// </summary>
		NotFound,
		
		/// <summary>
		/// The item exists but isn't of the type requested
		/// </summary>
		WrongType
	}
}
