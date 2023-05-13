using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Arrow.Application
{
    /// <summary>
    /// Represents a command line switch, which has a name and optional value.
    /// A switch begins with a forward slash (/) or a dash (-).
    /// Switch names are converted to lowercase to simplify processing
    /// </summary>
    [Serializable]
    public class CommandLineSwitch
    {
        private string m_Name;
        private string? m_Value;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="name">The name of the switch</param>
        /// <param name="value">The value for the switch. May be null</param>
        /// <exception cref="System.ArgumentNullException">name is null</exception>
        public CommandLineSwitch(string name, string? value)
        {
            if(name == null) throw new ArgumentNullException("name");

            m_Name = name.ToLower();
            m_Value = value;
        }

        /// <summary>
        /// The name of the switch
        /// </summary>
        public string Name
        {
            get{return m_Name;}
        }

        /// <summary>
        /// The value for the switch. May be null
        /// </summary>
        public string? Value
        {
            get{return m_Value;}
            set{m_Value = value;}
        }

        /// <summary>
        /// Checks that "Value" has a value. 
        /// If not it throws an exception with a user-friendly message
        /// </summary>
        public void EnsureValuePresent()
        {
            if(string.IsNullOrEmpty(m_Value))
            {
                throw new CommandLineSwitchException("no value for switch: " + m_Name);
            }
        }

        /// <summary>
        /// Checks that "Value" is empty. 
        /// If not it throws an exception with a user-friendly message
        /// </summary>
        public void EnsureNoValuePresent()
        {
            if(string.IsNullOrEmpty(m_Value) == false)
            {
                throw new CommandLineSwitchException("switch does not take a value: " + m_Name);
            }
        }

        /// <summary>
        /// Generates a hashcode from the name
        /// </summary>
        /// <returns>A hashcode</returns>
        public override int GetHashCode()
        {
            return m_Name.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the switch
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return m_Value == null ? m_Name : string.Format("{0}={1}", m_Name, m_Value);
        }

        /// <summary>
        /// Parses a switch
        /// </summary>
        /// <param name="text">The text to parse</param>
        /// <returns>A CommandSwitch instance</returns>
        /// <exception cref="System.ArgumentNullException">text is null</exception>
        /// <exception cref="System.FormatException">text is not a valid switch</exception>
        public static CommandLineSwitch Parse(string text)
        {
            if(TryParse(text, out var s) == false) throw new FormatException(text);

            return s;
        }

        /// <summary>
        /// Tries to parse a command line switch
        /// </summary>
        /// <param name="text">The text to parse</param>
        /// <param name="commandSwitch">A CommandSwitch instance on success, otherwise null</param>
        /// <returns>true if a swithc could be parsed, false otherwise</returns>
        /// <exception cref="System.ArgumentNullException">text is null</exception>
        public static bool TryParse(string text, [NotNullWhen(true)] out CommandLineSwitch? commandSwitch)
        {
            if(text == null) throw new ArgumentNullException("text");

            commandSwitch = null;

            // Check for boundary cases
            if(text == "") return false;
            if(text[0] != '/' && text[0] != '-') return false;

            text = text.Substring(1);

            string? name = null;
            string? value = null;

            // Extract the optional value
            int pivot = text.IndexOf(':');
            if(pivot == -1)
            {
                // It's a valueless switch
                name = text;
            }
            else
            {
                // There's a name and a value
                name = text.Substring(0, pivot);
                value = text.Substring(pivot + 1);
            }

            commandSwitch = new CommandLineSwitch(name, value);
            return true;
        }
    }
}
