using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Scripting
{
	/// <summary>
	/// Defined the behaviour for reading and writing to variables
	/// </summary>
	public interface IVariableManager : IVariableRead
	{
		/// <summary>
		/// Declares a new variable
		/// </summary>
		/// <param name="variableName">The name of the variable to declare</param>
		/// <param name="value">The value to initially assign to the variable</param>
		/// <exception cref="System.ArgumentNullException">if variableName is null</exception>
		/// <exception cref="Arrow.Scripting.DeclarationException">if a variable with the name is already declared</exception>
		void Declare(string variableName, object? value);

		/// <summary>
		/// Removes a declaration
		/// </summary>
		/// <param name="variableName">The name of the variable to undeclare</param>
		/// <returns>true if the variable was undeclared, otherwise false</returns>
		/// <exception cref="System.ArgumentNullException">if variableName is null</exception>
		bool Undeclare(string variableName);

		/// <summary>
		/// Assigns a value to an existing variable
		/// </summary>
		/// <param name="variableName">The name of the variable to assign to</param>
		/// <param name="value">The value for the variable</param>
		/// <exception cref="System.ArgumentNullException">if variableName is null</exception>
		/// <exception cref="Arrow.Scripting.DeclarationException">if a variable has not been declared</exception>
		void Assign(string variableName, object? value);

		/// <summary>
		/// Assigns a value to a variable. If it's not already declared it will be
		/// </summary>
		/// <param name="variableName">The name of the variable</param>
		/// <param name="value">The value to assign</param>
		/// <exception cref="System.ArgumentNullException">if variableName is null</exception>
		void AssignOrDeclare(string variableName, object? value);
	}
}
