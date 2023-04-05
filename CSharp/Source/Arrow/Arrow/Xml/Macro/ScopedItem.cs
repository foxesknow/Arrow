using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.Xml.Macro
{
	/// <summary>
	/// Represents a scoped item
	/// </summary>
	class ScopedItem
	{
		private object m_Value;
		private ScopedItemMode m_Mode;
			
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="value">The value the item was</param>
		/// <param name="mode">It's mode</param>
		public ScopedItem(object value, ScopedItemMode mode)
		{
			m_Value = value;
			m_Mode = mode;
		}
		
		/// <summary>
		/// The value of the scoped item
		/// </summary>
		public object Value
		{
			get{return m_Value;}
			set{m_Value = value;}
		}
		
		/// <summary>
		/// The mode of the scoped item
		/// </summary>
		public ScopedItemMode ScopedItemMode
		{
			get{return m_Mode;}
			internal set{m_Mode = value;}
		}
	}
}
