﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Execution
{
	/// <summary>
	/// Implements the IDisposable pattern by calling a method to do the actual disposing
	/// This saves having to write a class specificly to do this.
	/// The "disposer" delegate is guaranteed to only be called once.
	/// </summary>
	public class Disposer : IDisposable
	{
		private Action m_Disposer;
	
		/// <summary>
		/// Initializes the instace
		/// </summary>
		/// <param name="disposer">The method that will do the disposal</param>
		/// <exception cref="System.ArgumentNullException">disposer is null</exception>
		public Disposer(Action disposer)
		{
			if(disposer==null) throw new ArgumentNullException("disposer");
			m_Disposer=disposer;
		}
	
		void IDisposable.Dispose()
		{
			var disposer=m_Disposer;
			m_Disposer=null;
			if(disposer!=null) disposer();
			
		}
	}
}
