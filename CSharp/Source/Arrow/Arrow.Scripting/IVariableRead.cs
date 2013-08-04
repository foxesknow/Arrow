using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Scripting
{
	/// <summary>
	/// Defined the behaviour for reading variables
	/// </summary>
	public interface IVariableRead
	{
		/// <summary>
		/// Returns the value of a variable
		/// </summary>
		/// <param name="variableName">The name of the variable to lookup</param>
		/// <returns>The value of the variable</returns>
		/// <exception cref="System.ArgumentNullException">if variableName is null</exception>
		/// <exception cref="Arrow.Scripting.VariableNotFoundException">if the variable is not defined</exception>
		object GetVariable(string variableName);
		
		/// <summary>
		/// Attempts to get the value of a variable
		/// </summary>
		/// <param name="variableName">The name of the variable to lookup</param>
		/// <param name="result">On success the value of the variable</param>
		/// <returns>true on success, false if the variable could not be found</returns>
		/// <exception cref="System.ArgumentNullException">if variableName is null</exception>
		bool TryGetVariable(string variableName, out object result);
		
		/// <summary>
		/// Checks to see if a variable is present
		/// </summary>
		/// <param name="variableName">The name of the variable to lookup</param>
		/// <returns>true if the variable is present, otherwise false</returns>
		/// <exception cref="System.ArgumentNullException">if variableName is null</exception>
		bool IsDeclared(string variableName);
	}
}
