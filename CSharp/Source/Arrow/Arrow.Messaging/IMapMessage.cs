using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging
{
	/// <summary>
	/// Defines the behavior of a message that maps string
	/// keys to values
	/// </summary>
	public interface IMapMessage : IMessage
	{
		/// <summary>
		/// Gets a boolean value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The bool with the specified name</returns>
		bool GetBool(string name);
		
		/// <summary>
		/// Sets a bool value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		void SetBool(string name, bool value);
		
		/// <summary>
		/// Gets a byte value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The byte with the specified name</returns>
		byte GetByte(string name);
		
		/// <summary>
		/// Sets a byte value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		void SetByte(string name, byte value);
		
		/// <summary>
		/// Gets a char value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The char with the specified name</returns>		
		char GetChar(string name);
		
		/// <summary>
		/// Sets a char value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		void SetChar(string name, char value);
		
		/// <summary>
		/// Gets a short value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The short with the specified name</returns>
		short GetShort(string name);
		
		/// <summary>
		/// Sets a short value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		void SetShort(string name, short value);
		
		/// <summary>
		/// Gets a int value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The int with the specified name</returns>
		int GetInt(string name);
		
		/// <summary>
		/// Sets a int value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		void SetInt(string name, int value);
		
		/// <summary>
		/// Gets a long value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The long with the specified name</returns>
		long GetLong(string name);
		
		/// <summary>
		/// Sets a long value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		void SetLong(string name, long value);
		
		/// <summary>
		/// Gets a string value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The byte with the specified name</returns>
		string GetString(string name);
		
		/// <summary>
		/// Sets a string value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		void SetString(string name, string value);
		
		/// <summary>
		/// Gets an object value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The object with the specified name</returns>
		object GetObject(string name);
		
		/// <summary>
		/// Sets a object value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		void SetObject(string name, object value);
		
		/// <summary>
		/// Gets a byte array value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The byte array with the specified name</returns>
		byte[] GetBytes(string name);
		
		/// <summary>
		/// Sets a byte array value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		void SetBytes(string name, byte[] value);
		
		/// <summary>
		/// Gets a float value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The float with the specified name</returns>
		float GetFloat(string name);
		
		/// <summary>
		/// Sets a float value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		void SetFloat(string name, float value);
		
		/// <summary>
		/// Gets a double value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The double with the specified name</returns>
		double GetDouble(string name);
		
		
		/// <summary>
		/// Sets a double value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		void SetDouble(string name, double value);
		
		/// <summary>
		/// Returns the names of the values held in the message
		/// </summary>
		IEnumerable<string> Names{get;}
		
		/// <summary>
		/// Indicates if a value is present
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>true if the map contains the named value, otherwise false</returns>
		bool Contains(string name);
	}
}
