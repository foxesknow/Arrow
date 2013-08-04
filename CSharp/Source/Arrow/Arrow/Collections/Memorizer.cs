using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Collections
{
	/// <summary>
	/// Provides a cache of mappable values.
	/// </summary>
	/// <typeparam name="FROM">The type of the value to map from</typeparam>
	/// <typeparam name="TO">The type of the value to map to</typeparam>
	/// 
	/// <remarks>
	/// If the value (FROM) does not exist in the map a lookup function
	/// is called to get the value (TO) and it is added to the cache.
	/// Subsequent lookups will return the cached value.
	/// 
	/// For example, the method below uses the memorizer to calculate Fibonnaci numbers.
	/// By using the memorizer it is able to easily cache values that were previously
	/// calculated. For small calls, such as Fib(3) this doesn't make a lot of difference,
	/// but for large calls, such as Fib(50) it provides a huge speed improvement.
	/// 
	/// <code>
	/// <![CDATA[
	/// static long Fib(long number)
	///	{
	///		Converter<long,long> fib=null;
	///		
	///		fib=Memorizer<long,long>.Memorize(delegate(long n)
	///		{
	///			if(n==0) return 0;
	///			if(n==1) return 1;
	///			
	///			return fib(n-1)+fib(n-2);
	///		});
	///		
	///		return fib(number);
	///	}
	///	]]>
	/// </code>
	/// </remarks>
	[Serializable]
	public class Memorizer<FROM,TO> : IEnumerable<KeyValuePair<FROM,TO>>
	{
		private Dictionary<FROM,TO> m_Values=null;
		
		private Func<FROM,TO> m_LookupFunction;
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="lookupFunction">A function that can map from one value to another</param>
		/// <exception cref="System.ArgumentNullException">lookupFunction is null</exception>
		public Memorizer(Func<FROM,TO> lookupFunction) : this(lookupFunction,null)
		{
		}
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="lookupFunction">A function that can map from one value to another</param>
		/// <param name="equalityComparer">A comparer that will check for equality of "from" values</param>
		/// <exception cref="System.ArgumentNullException">lookupFunction is null</exception>
		public Memorizer(Func<FROM,TO> lookupFunction, IEqualityComparer<FROM> equalityComparer)
		{
			if(lookupFunction==null) throw new ArgumentNullException("lookupFunction");
			
			m_LookupFunction=lookupFunction;
			m_Values=new Dictionary<FROM,TO>(equalityComparer);
		}
		
		/// <summary>
		/// Retrieves a value from the memorizer, doing a lookup if the value has not yet been requested
		/// </summary>
		/// <param name="from">The value to map from</param>
		/// <returns>The value that "from" maps to</returns>
		public TO Lookup(FROM from)
		{
			TO to=default(TO);
			
			if(m_Values.TryGetValue(from,out to)==false)
			{
				// The value hasn't been looked up yet, so fetch in
				to=m_LookupFunction(from);
				m_Values.Add(from,to);
			}
			
			return to;
		}
		
		
		/// <summary>
		/// The number of cached values
		/// </summary>
		/// <value>The numbber of cached values</value>
		public int Count
		{
			get{return m_Values.Count;}
		}
		
		/// <summary>
		/// Removes all cached values
		/// </summary>
		public void Clear()
		{
			m_Values.Clear();
		}
		
		/// <summary>
		/// Provides a quick and easy way to create a memorizer that
		/// returns a delegate with a matching signature that can perform a lookup
		/// </summary>
		/// <param name="lookupFunction">A function that can to the lookup</param>
		/// <returns>A function with the same signiture as the lookup function but which remembers the results of previous calls</returns>
		public static Func<FROM,TO> Memorize(Func<FROM,TO> lookupFunction)
		{
			return new Memorizer<FROM,TO>(lookupFunction).Lookup;
		}

		/// <summary>
		/// Returns an enumerator to previously requested values
		/// </summary>
		/// <returns>An enumerator to requested values</returns>
		public IEnumerator<KeyValuePair<FROM,TO>> GetEnumerator()
		{
			foreach(KeyValuePair<FROM,TO> e in m_Values)
			{
				yield return e;
			}
		}

		/// <summary>
		/// Returns an enumerator to previously requested values
		/// </summary>
		/// <returns>An enumerator to requested values</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
