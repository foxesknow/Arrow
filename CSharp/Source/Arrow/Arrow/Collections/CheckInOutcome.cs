using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Collections
{
	/// <summary>
	/// Indicates the outcome of checking data into the ReferenceCountedResource
	/// </summary>
	public enum CheckInOutcome
	{
		/// <summary>
		/// The checked in data is not currently managed
		/// </summary>
		NotManaged,
		
		/// <summary>
		/// The checked in data is still in use within the system
		/// </summary>
		StillInUse,
		
		/// <summary>
		/// The checked in data is no longer in use and should be released
		/// </summary>
		NotInUse
	}
}
