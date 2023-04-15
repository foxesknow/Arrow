using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable

namespace Arrow.Collections.Extensions
{
	/// <summary>
	/// Dictionary extension methods
	/// </summary>
	public static class DictionaryExtensions
	{
		/// <summary>
		/// Tries to look up a value from a dictionary, returning a default if not found
		/// </summary>
		/// <typeparam name="TKey">The type of keys in the dictionary</typeparam>
		/// <typeparam name="TValue">The type of values in the dictionary</typeparam>
		/// <param name="dictionary">The dictionary to check</param>
		/// <param name="key">The key to lookup</param>
		/// <returns>The value for the key, or default(TValue) if not found</returns>
		public static TValue GetValueOrDefault<TKey,TValue>(this IDictionary<TKey,TValue> dictionary, TKey key)
		{
			return dictionary.GetValueOrDefault(key,default(TValue));
		}
		
		/// <summary>
		/// Tries to look up a value from a dictionary, returning a default if not found
		/// </summary>
		/// <typeparam name="TKey">The type of keys in the dictionary</typeparam>
		/// <typeparam name="TValue">The type of values in the dictionary</typeparam>
		/// <param name="dictionary">The dictionary to check</param>
		/// <param name="key">The key to lookup</param>
		/// <param name="defaultValue">The value to return if key does not exist</param>
		/// <returns>The value for the key, or default(TValue) if not found</returns>
		public static TValue GetValueOrDefault<TKey,TValue>(this IDictionary<TKey,TValue> dictionary, TKey key, TValue defaultValue)
		{
			TValue value;
			if(dictionary.TryGetValue(key,out value))
			{
				return value;
			}
			
			return defaultValue;
		}

		/// <summary>
		/// Adds the data in "additionalDictionary" to the dictionary, replacing any existing values
		/// </summary>
		/// <typeparam name="TKey">The type of keys in the dictionary</typeparam>
		/// <typeparam name="TValue">The type of values in the dictionary</typeparam>
		/// <param name="dictionary">The dictionary to extend</param>
		/// <param name="additionalValues">The values to add to the dictionary</param>
		public static void Extend<TKey,TValue>(this IDictionary<TKey,TValue> dictionary, IDictionary<TKey,TValue> additionalValues)
		{
			if(additionalValues == null) throw new ArgumentNullException("additionalValues");

			foreach(var pair in additionalValues)
			{
				dictionary[pair.Key] = pair.Value;
			}
		}
	}
}
