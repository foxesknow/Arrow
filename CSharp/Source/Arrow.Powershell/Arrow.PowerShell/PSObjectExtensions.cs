using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell
{
	/// <summary>
	/// Useful extension methods for PSObject
	/// </summary>
	public static class PSObjectExtensions
	{
		/// <summary>
		/// Specified which properties will be shown in the host by default
		/// </summary>
		/// <param name="psObject">The object to set the display prop</param>
		/// <param name="propertyNames">The properties to display, by default</param>
		public static void SetDisplayProperties(this PSObject psObject, params string[] propertyNames)
		{
			var properties=new PSPropertySet("DefaultDisplayPropertySet",propertyNames);
			var memberSet=new PSMemberSet("PSStandardMembers",new[]{properties});
			psObject.Members.Add(memberSet);
		}
	}
}
