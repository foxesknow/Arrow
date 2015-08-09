using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell
{
    /// <summary>
    /// Checks that a string argument is not null or whitespace
    /// </summary>
	[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]
	public class ValidateNotNullOrWhitespaceAttribute : ValidateArgumentsAttribute
    {
		protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
		{
			if(arguments==null) throw new ValidationMetadataException("argument is null");

			// If it's not a string then don't worry about it
			string value=arguments as string;
			if(value!=null && string.IsNullOrWhiteSpace(value))
			{
				throw new ValidationMetadataException("string is whitespace");
			}
		}

		public override string ToString()
		{
			return "[ValidateNotNullOrWhitespace]";
		}
	}
}
