using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.Xml.Macro
{
	/// <summary>
	/// The mode for a scoped item
	/// </summary>
	public enum ScopedItemMode
	{
		/// <summary>
		/// The items mode is unknown
		/// </summary>
		Unknown,
	
		/// <summary>
		/// The item is readable and writable
		/// </summary>
		ReadWrite,
		
		/// <summary>
		/// The item is read only
		/// </summary>
		ReadOnly
	}
}
