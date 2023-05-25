using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections;

namespace Arrow.Scripting
{
	/// <summary>
	/// Provides a lightweight implementation of a read/write variable scope.
	/// The class supports multiple readers, as long as scope is not modified.
	/// </summary>
    public sealed class LightweightScope : IVariableManager
    {
        private readonly Dictionary<string, object?> m_Variables;

        /// <summary>
        /// Initializes the instance in case insensitive mode
        /// </summary>
        public LightweightScope() : this(CaseMode.Insensitive)
        {

        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="caseMode">The case mode to use for the variable names</param>
        public LightweightScope(CaseMode caseMode)
        {
            IEqualityComparer<string>? comparer = null;
            if(caseMode == CaseMode.Insensitive) comparer = StringComparer.OrdinalIgnoreCase;

            m_Variables = new(comparer);
        }

        /// <summary>
        /// Returns the value of a variable
        /// </summary>
        /// <param name="variableName">The name of the variable to lookup</param>
        /// <returns>The value of the variable</returns>
        /// <exception cref="System.ArgumentNullException">if variableName is null</exception>
        /// <exception cref="Arrow.Scripting.VariableNotFoundException">if the variable is not defined</exception>
        public object? GetVariable(string variableName)
        {
            if(variableName == null) throw new ArgumentNullException("variableName");

            if(m_Variables.TryGetValue(variableName, out var value) == false)
            {
                throw new VariableNotFoundException(variableName);
            }

            return value;
        }

        /// <summary>
        /// Attempts to get the value of a variable
        /// </summary>
        /// <param name="variableName">The name of the variable to lookup</param>
        /// <param name="result">On success the value of the variable</param>
        /// <returns>true on success, false if the variable could not be found</returns>
        /// <exception cref="System.ArgumentNullException">if variableName is null</exception>
        public bool TryGetVariable(string variableName, out object? result)
        {
            return m_Variables.TryGetValue(variableName, out result);
        }

        /// <summary>
        /// Checks to see if a variable is present
        /// </summary>
        /// <param name="variableName">The name of the variable to lookup</param>
        /// <returns>true if the variable is present, otherwise false</returns>
        /// <exception cref="System.ArgumentNullException">if variableName is null</exception>
        public bool IsDeclared(string variableName)
        {
            if(variableName == null) throw new ArgumentNullException("variableName");

            return m_Variables.ContainsKey(variableName);
        }


        /// <summary>
        /// Declares a new variable
        /// </summary>
        /// <param name="variableName">The name of the variable to declare</param>
        /// <param name="value">The value to initially assign to the variable</param>
        /// <exception cref="System.ArgumentNullException">if variableName is null</exception>
        /// <exception cref="Arrow.Scripting.DeclarationException">if a variable with the name is already declared</exception>
        public void Declare(string variableName, object? value)
        {
            if(variableName == null) throw new ArgumentNullException("variableName");
            if(m_Variables.ContainsKey(variableName)) throw new DeclarationException("already declared: " + variableName);

            m_Variables.Add(variableName, value);
        }

        /// <summary>
        /// Removes a declaration
        /// </summary>
        /// <param name="variableName">The name of the variable to undeclare</param>
        /// <returns>true if the variable was undeclared, otherwise false</returns>
        /// <exception cref="System.ArgumentNullException">if variableName is null</exception>
        public bool Undeclare(string variableName)
        {
            if(variableName == null) throw new ArgumentNullException("variableName");

            return m_Variables.Remove(variableName);
        }

        /// <summary>
        /// Assigns a value to an existing variable
        /// </summary>
        /// <param name="variableName">The name of the variable to assign to</param>
        /// <param name="value">The value for the variable</param>
        /// <exception cref="System.ArgumentNullException">if variableName is null</exception>
        /// <exception cref="Arrow.Scripting.DeclarationException">if a variable has not been declared</exception>
        public void Assign(string variableName, object? value)
        {
            if(variableName == null) throw new ArgumentNullException("variableName");
            
            if(m_Variables.ContainsKey(variableName) == false) throw new DeclarationException("not declared: " + variableName);
            m_Variables[variableName] = value;
        }

        /// <summary>
        /// Assigns a value to a variable. If it's not already declared it will be
        /// </summary>
        /// <param name="variableName">The name of the variable</param>
        /// <param name="value">The value to assign</param>
        /// <exception cref="System.ArgumentNullException">if variableName is null</exception>
        public void AssignOrDeclare(string variableName, object? value)
        {
            if(variableName == null) throw new ArgumentNullException("variableName");

            m_Variables[variableName] = value;
        }
    }
}
