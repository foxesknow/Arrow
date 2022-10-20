using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Arrow.Collections
{
	/// <summary>
	/// Managed named resources using reference counting
	/// </summary>
	/// <typeparam name="K">The key of the resource</typeparam>
	/// <typeparam name="V">The type of the resource</typeparam>
	public class ReferenceCountedResource<K,V> where V:class where K : notnull
	{
		private readonly object m_SyncRoot=new object();
		
		private readonly Dictionary<K,ReferenceData> m_Data;
		
		/// <summary>
		/// Initializes the instance using the default key comparer
		/// </summary>
		public ReferenceCountedResource() : this(null)
		{
		}
		
		/// <summary>
		/// Initializes the instance using a specific key comparer
		/// </summary>
		/// <param name="equalityComparer">The key comparer. If null then the default is used</param>
		public ReferenceCountedResource(IEqualityComparer<K>? equalityComparer)
		{
			m_Data=new Dictionary<K,ReferenceData>(equalityComparer);
		}
		
		/// <summary>
		/// Adds a new value to the store and starts to reference count it
		/// </summary>
		/// <param name="key">The key of the data to store</param>
		/// <param name="resource">The resource to reference count</param>
		public void Add(K key, V resource)
		{
			ReferenceData data=new ReferenceData(resource);
			
			lock(m_SyncRoot)
			{
				m_Data.Add(key,data);
			}
		}
		
		/// <summary>
		/// Attempts to checkout a resource
		/// </summary>
		/// <param name="key">The key of the resource to checkout</param>
		/// <param name="resource">On success the requested resource</param>
		/// <returns>true if the resource was checked out, otherwise false</returns>
		public bool TryCheckout(K key, [MaybeNullWhen(false)] out V resource)
		{
			lock(m_SyncRoot)
			{
				if(m_Data.TryGetValue(key,out var data))
				{
					data.AddRef();
					resource=data.Value;
					return true;
				}
				
				resource=default!;
				return false;
			}
		}
		
		/// <summary>
		/// Checks in a resource. If the resource is no longer in use it
		/// is the job of the caller to dispose or close it
		/// </summary>
		/// <param name="value">The resource to check in</param>
		/// <returns>StillInUse if there are still references to the resource, otherwise NotInUse</returns>
		public CheckInOutcome Checkin(V value)
		{
			lock(m_SyncRoot)
			{
				foreach(var pair in m_Data)
				{
					ReferenceData data=pair.Value;
					
					if(data.Value==value)
					{
						if(data.Release()==0) 
						{
							m_Data.Remove(pair.Key);
							return CheckInOutcome.NotInUse;
						}
						else
						{
							return CheckInOutcome.StillInUse;
						}
					}
				}
			}
			
			return CheckInOutcome.NotManaged;
		}
		
		/// <summary>
		/// Determines if a resource is present
		/// </summary>
		/// <param name="key">The key of the resource</param>
		/// <returns>true if the resource is present, false otherwise</returns>
		public bool Contains(K key)
		{
			lock(m_SyncRoot)
			{
				return m_Data.ContainsKey(key);
			}
		}
		
		class ReferenceData
		{
			private long m_ReferenceCount;
		
			public ReferenceData(V value)
			{
				this.Value=value;
				AddRef();
			}
		
			public V Value{get;private set;}
			
			public long AddRef()
			{
				return ++m_ReferenceCount;
			}
			
			public long Release()
			{
				return --m_ReferenceCount;
			}
		}
	}
}
