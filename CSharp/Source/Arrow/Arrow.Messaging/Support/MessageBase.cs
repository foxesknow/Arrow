using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging.Support
{
	/// <summary>
	/// Provides a reasonable default implementation of a message
	/// for use in specific implementations that do not have a concreate
	/// message class, such as 29West
	/// </summary>
	[Serializable]
	public class MessageBase : IMessage
	{
		private Dictionary<string,object> m_Properties;
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		protected MessageBase()
		{
		}
	
		/// <summary>
		/// Acknowledges a message with the underlying messaging system
		/// if supported and enabled.
		/// </summary>
		public virtual void Acknowledge()
		{
			// Does nothing
		}

		/// <summary>
		/// Indicates if the message is readonly
		/// </summary>
		public virtual bool ReadOnly
		{
			get{return false;}
		}

		/// <summary>
		/// The message type
		/// </summary>
		public virtual string MessageType{get; set;}

		/// <summary>
		/// The message id
		/// </summary>
		public virtual string MessageID{get; set;}

		/// <summary>
		/// The correlation id of the message
		/// </summary>
		public virtual string CorrelationID{get; set;}

        /// <summary>
        /// Sets a message property
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="value">The value of the property</param>
        public virtual void SetProperty(string name, object value)
        {
            if(name == null) throw new ArgumentNullException("name");

            if(m_Properties == null) m_Properties = new Dictionary<string, object>();
            m_Properties[name] = value;
        }

        /// <summary>
        /// Gets a property
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <returns>The value of the property, or null if the property does not exist</returns>
        public virtual object GetProperty(string name)
        {
            if(name == null) throw new ArgumentNullException("name");
            if(m_Properties == null) return null;

            object value;
            m_Properties.TryGetValue(name, out value);

            return value;
        }

        /// <summary>
        /// Returns the names of all message properties
        /// </summary>
        public virtual IEnumerable<string> PropertyNames
		{
			get
			{
				if(m_Properties == null) return new string[0];
				return m_Properties.Keys;
			}
		}

		/// <summary>
		/// Checks to see if a property is present
		/// </summary>
		/// <param name="name">The name of the property</param>
		/// <returns>true if the property exists, false otherwise</returns>
		public virtual bool ContainsProperty(string name)
		{
            if(name == null) throw new ArgumentNullException("name");
            if(m_Properties == null) return false;

            return m_Properties.ContainsKey(name);
        }

		/// <summary>
		/// Removes all properties
		/// </summary>
		public virtual void ClearProperties()
		{
			m_Properties = null;
		}
	}
}
