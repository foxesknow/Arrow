using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging
{
	/// <summary>
	/// Base definition of all messages
	/// </summary>
	public interface IMessage
	{
		/// <summary>
		/// Acknowledges a message with the underlying messaging system
		/// if supported and enabled.
		/// </summary>
		void Acknowledge();
		
		/// <summary>
		/// Indicates if the message is readonly
		/// </summary>
		bool ReadOnly{get;}
		
		/// <summary>
		/// The message type
		/// </summary>
		string MessageType{get; set;}
		
		/// <summary>
		/// The message id
		/// </summary>
		string MessageID{get; set;}
		
		/// <summary>
		/// The correlation id of the message
		/// </summary>
		string CorrelationID{get; set;}
		
		/// <summary>
		/// Sets a message property
		/// </summary>
		/// <param name="name">The name of the property</param>
		/// <param name="value">The value of the property</param>
		void SetProperty(string name, object value);
		
		/// <summary>
		/// Gets a property
		/// </summary>
		/// <param name="name">The name of the property</param>
		/// <returns>The value of the property, or null if the property does not exist</returns>
		object GetProperty(string name);
		
		/// <summary>
		/// Returns the names of all message properties
		/// </summary>
		IEnumerable<string> PropertyNames{get;}
		
		/// <summary>
		/// Checks to see if a property is present
		/// </summary>
		/// <param name="name">The name of the property</param>
		/// <returns>true if the property exists, false otherwise</returns>
		bool ContainsProperty(string name);
		
		/// <summary>
		/// Removes all properties
		/// </summary>
		void ClearProperties();
	}
}
