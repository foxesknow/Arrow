using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging.Support
{
	/// <summary>
	/// Provides a reasonable default implementation of a text message
	/// for use in specific implementations that do not have a concreate
	/// message class, such as 29West
	/// </summary>
	[Serializable]
	public class TextMessage : MessageBase, ITextMessage
	{
		private readonly string m_Text;
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="text">The text message that makes up the instance</param>
		public TextMessage(string text)
		{
			m_Text = text;
		}
	
		/// <summary>
		/// The text in the message
		/// </summary>
		public virtual string Text
		{
			get{return m_Text;}
		}
	}
}
