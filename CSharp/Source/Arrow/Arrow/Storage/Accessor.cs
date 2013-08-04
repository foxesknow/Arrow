using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Arrow.Storage
{
	/// <summary>
	/// Base class for any system that provides access to a named external
	/// resource that can be treated as a file
	/// </summary>
	public abstract partial class Accessor
	{
		private readonly Uri m_Uri;

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="uri">The location of the item to access</param>
		protected Accessor(Uri uri)
		{
			if(uri==null) throw new ArgumentNullException("uri");
			m_Uri=uri;
		}

		/// <summary>
		/// The item to access
		/// </summary>
		public Uri Uri
		{
			get{return m_Uri;}
		}

		/// <summary>
		/// Indicates if data can be read from the accessor
		/// </summary>
		/// <returns>true if data can be read, false if not</returns>
		public virtual bool CanRead
		{
			get{return true;}
		}

		/// <summary>
		/// Attempts to open a stream to the resource at a given uri.
		/// It is the callers responsibility to close the returned stream.
		/// </summary>
		/// <returns>A stream to the referenced resource</returns>
		/// <exception cref="System.IO.IOException">The resource could not be opened</exception>
		public abstract Stream OpenRead();

		/// <summary>
		/// Indicates if data can be written by the accessor
		/// </summary>
		public virtual bool CanWrite
		{
			get{return false;}
		}

		/// <summary>
		/// Attempts to open a stream to the resource at a given uri.
		/// It is the callers responsibility to close the returned stream.
		/// </summary>
		/// <returns>A stream to the referenced resource</returns>
		/// <exception cref="System.IO.IOException">The resource could not be opened</exception>
		public virtual Stream OpenWrite()
		{
			throw new NotImplementedException("OpenWrite");
		}

		/// <summary>
		/// Indicates if the accessor supports the Exists() method.
		/// </summary>
		public virtual bool CanExists
		{
			get{return false;}
		}
		
		/// <summary>
		/// Determines if the resource at a given uri exists.
		/// </summary>
		/// <returns>true if the resource exists, otherwise false</returns>
		public virtual bool Exists()
		{
			throw new NotSupportedException("Exists");
		}


		/// <summary>
		/// Validates the scheme of a uri
		/// </summary>
		/// <param name="required">The required scheme</param>
		/// <param name="uri">The uri to check</param>
		/// <exception cref="System.IO.IOException">The uri does not use the required scheme</exception>
		protected void ValidateScheme(Uri uri, string required)
		{
			string scheme=uri.Scheme;
			
			if(string.Compare(required,scheme,true)!=0)
			{
				string message=string.Format("expected {0}, received {1}",required,scheme);
				throw new IOException(message);
			}
		}

		/// <summary>
		/// Renders the accessor
		/// </summary>
		/// <returns>A string representation of the instance</returns>
		public override string ToString()
		{
			return m_Uri.ToString();
		}

		/// <summary>
		/// A hashcode for the accessor
		/// </summary>
		/// <returns>A hashcode based on the Uri property</returns>
		public override int GetHashCode()
		{
			return m_Uri.GetHashCode();
		}

		/// <summary>
		/// Compares two accessors for equality
		/// </summary>
		/// <param name="obj">The right hand side of the comparison</param>
		/// <returns>true if the two instances refer to the same item, otherwise false</returns>
		public override bool Equals(object obj)
		{
			if(obj==null) return false;

			Accessor rhs=obj as Accessor;
			if(rhs==null) return false;

			return m_Uri==rhs.m_Uri;
		}		
	}
}
