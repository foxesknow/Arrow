using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Collections
{
	/// <summary>
	/// Allows something to expose the Add() functionality of a collection
	/// without exposing the whole collection
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IValueAdder<T>
	{
		/// <summary>
		/// Adds a value to some store
		/// </summary>
		/// <param name="item">The value to add</param>
		void Add(T item);
	}
}
