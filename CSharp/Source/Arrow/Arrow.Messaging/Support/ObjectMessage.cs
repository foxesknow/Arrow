using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging.Support
{
	/// <summary>
	/// Provides a reasonable default implementation of an object message
	/// for use in specific implementations that do not have a concreate
	/// message class, such as 29West
	/// </summary>
	[Serializable]
	public class ObjectMessage : MessageBase, IObjectMessage
	{
		private readonly object m_Object;
		
		/// <summary>
		/// Initializes the intstance
		/// </summary>
		/// <param name="theObject">The object the message holds</param>
		public ObjectMessage(object theObject)
		{
			m_Object=theObject;
		}
		
		
		#region IObjectMessage Members

		/// <summary>
		/// The object held in the message
		/// </summary>
		public object TheObject
		{
			get{return m_Object;}
		}

		#endregion
	}
}
