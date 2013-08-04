using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.Logging
{
	/// <summary>
	/// Optional interface for log implementations that allows them to initialize themselves.
	/// It's seperate from the ILog interface so that user of ILog don't see the Initialize method
	/// in their Intellisense
	/// </summary>
	public interface ILogInitializer
	{
		/// <summary>
		/// Initializes the log
		/// </summary>
		/// <param name="name">The name of the log</param>
		void Initialize(string name);
	}
}
