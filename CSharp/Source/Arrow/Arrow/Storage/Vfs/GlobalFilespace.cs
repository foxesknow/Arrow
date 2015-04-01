using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Collections;

namespace Arrow.Storage.Vfs
{
	/// <summary>
	/// A system wide collection of file spaces
	/// </summary>
	public static class GlobalFilespace
	{
		private static object s_SyncRoot=new object();
		private static readonly Dictionary<string,VirtualFileSystem> s_Filespaces=new Dictionary<string,VirtualFileSystem>(IgnoreCaseEqualityComparer.Instance);

		/// <summary>
		/// Returns an existing filespace, or creates one with the specified name and returns it
		/// </summary>
		/// <param name="filespaceName">The name of the filespace</param>
		/// <returns>The requested filespace</returns>
		public static VirtualFileSystem GetOrCreate(string filespaceName)
		{
			if(filespaceName==null) throw new ArgumentNullException("filespaceName");

			string normalizedName=filespaceName.Trim();
			if(string.IsNullOrEmpty(normalizedName)) throw new ArgumentException("invalid filespace name: "+filespaceName);

			lock(s_SyncRoot)
			{
				VirtualFileSystem filespace=null;
				if(s_Filespaces.TryGetValue(normalizedName,out filespace)==false)
				{
					filespace=new VirtualFileSystem();
					s_Filespaces.Add(normalizedName,filespace);
				}

				return filespace;
			}
		}

		/// <summary>
		/// Trys to get an existing filespace
		/// </summary>
		/// <param name="filespaceName">The name of the file space to fetch</param>
		/// <param name="filespace">On success the filespace, otherwise null</param>
		/// <returns>true if the filespace was found, otherwise false</returns>
		public static bool TryGetFilespace(string filespaceName, out VirtualFileSystem filespace)
		{
			if(filespaceName==null) throw new ArgumentNullException("filespaceName");

			string normalizedName=filespaceName.Trim();
			if(string.IsNullOrEmpty(normalizedName)) throw new ArgumentException("invalid filespace name: "+filespaceName);

			lock(s_SyncRoot)
			{
				return s_Filespaces.TryGetValue(normalizedName,out filespace);
			}
		}

		/// <summary>
		/// Returns the names of all the registered file spaces
		/// </summary>
		/// <returns>The names of all the registered file spaces</returns>
		public static IList<string> GetNames()
		{
			lock(s_SyncRoot)
			{
				return s_Filespaces.Keys.ToArray();
			}
		}

		/// <summary>
		/// Removes a registered file soace
		/// </summary>
		/// <param name="filespaceName">The name of the file space to remove</param>
		/// <returns>true if the file space was removed, otherwise false</returns>
		public static bool Remove(string filespaceName)
		{
			if(filespaceName==null) throw new ArgumentNullException("filespaceName");

			string normalizedName=filespaceName.Trim();
			if(string.IsNullOrEmpty(normalizedName)) throw new ArgumentException("invalid filespace name: "+filespaceName);

			lock(s_SyncRoot)
			{
				return s_Filespaces.Remove(normalizedName);
			}
		}

		/// <summary>
		/// Removes all file spaces
		/// </summary>
		public static void Clear()
		{
			lock(s_SyncRoot)
			{
				s_Filespaces.Clear();
			}
		}
	}
}
