using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Arrow.Storage
{
	public static partial class StorageManager
	{
		/// <summary>
		/// checks if the underlying item can be read
		/// </summary>
		/// <param name="uri">The item to check</param>
		/// <returns>true if if can be read, otherwise false</returns>
		public static bool CanRead(Uri uri)
		{
			var accessor=Get(uri);
			return accessor.CanRead;
		}

		/// <summary>
		/// Opens an item
		/// </summary>
		/// <param name="uri">The location of the item</param>
		/// <returns>A stream to the item</returns>
		public static Stream OpenRead(Uri uri)
		{
			var accessor=Get(uri);
			return accessor.OpenRead();
		}

		/// <summary>
		/// Indicates if the item can be written to
		/// </summary>
		/// <param name="uri">The location of the item</param>
		/// <returns>true if the item can be written to, otherwise false</returns>
		public static bool CanWrite(Uri uri)
		{
			var accessor=Get(uri);
			return accessor.CanWrite;
		}

		/// <summary>
		/// Opens a stream for writing to an item
		/// </summary>
		/// <param name="uri">The location of the item</param>
		/// <returns>A stream to the item</returns>
		public static Stream OpenWrite(Uri uri)
		{
			var accessor=Get(uri);
			return accessor.OpenWrite();
		}

		/// <summary>
		/// Indicates if the item can be checked for existence
		/// </summary>
		/// <param name="uri">The location of the item</param>
		/// <returns>true if the item can be checked for existence, otherwise false</returns>
		public static bool CanExists(Uri uri)
		{
			var accessor=Get(uri);
			return accessor.CanExists;
		}
		
		/// <summary>
		/// Indicate if the item exits
		/// </summary>
		/// <param name="uri">The item to check</param>
		/// <returns>true if the item exists, otherwise false</returns>
		public static bool Exists(Uri uri)
		{
			var accessor=Get(uri);
			return accessor.Exists();
		}
	}
}
