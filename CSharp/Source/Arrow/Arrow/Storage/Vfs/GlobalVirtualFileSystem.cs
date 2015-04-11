using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Collections;

namespace Arrow.Storage.Vfs
{
	/// <summary>
	/// A system wide collection of virtual file systems
	/// </summary>
	public static class GlobalVirtualFileSystem
	{
		private static object s_SyncRoot=new object();
		private static readonly Dictionary<string,VirtualFileSystem> s_FileSystems=new Dictionary<string,VirtualFileSystem>(IgnoreCaseEqualityComparer.Instance);

		/// <summary>
		/// Returns an existing filespace, or creates one with the specified name and returns it
		/// </summary>
		/// <param name="filesystemName">The name of the filespace</param>
		/// <returns>The requested filespace</returns>
		public static VirtualFileSystem GetOrCreate(string filesystemName)
		{
			VirtualFileSystem.ValidateName(filesystemName);

			lock(s_SyncRoot)
			{
				VirtualFileSystem filespace=null;
				if(s_FileSystems.TryGetValue(filesystemName,out filespace)==false)
				{
					filespace=new VirtualFileSystem();
					s_FileSystems.Add(filesystemName,filespace);
				}

				return filespace;
			}
		}

		/// <summary>
		/// Registers an existing file system with the global system
		/// </summary>
		/// <param name="filesystemName">The name of the file system to register</param>
		/// <param name="filesystem">The file system to register</param>
		public static void Register(string filesystemName, VirtualFileSystem filesystem)
		{
			VirtualFileSystem.ValidateName(filesystemName);
			if(filesystem==null) throw new ArgumentNullException("filesystem");

			lock(s_SyncRoot)
			{
				if(s_FileSystems.ContainsKey(filesystemName))
				{
					throw new IOException("file system already exists: "+filesystemName);
				}

				s_FileSystems.Add(filesystemName,filesystem);
			}
		}

		/// <summary>
		/// Trys to get an existing system
		/// </summary>
		/// <param name="filesystemName">The name of the file system to fetch</param>
		/// <param name="filespace">On success the file system, otherwise null</param>
		/// <returns>true if the file system was found, otherwise false</returns>
		public static bool TryGetFilesystem(string filesystemName, out VirtualFileSystem filespace)
		{
			VirtualFileSystem.ValidateName(filesystemName);

			lock(s_SyncRoot)
			{
				return s_FileSystems.TryGetValue(filesystemName,out filespace);
			}
		}

		/// <summary>
		/// Returns the names of all the registered file systems
		/// </summary>
		/// <returns>The names of all the registered file systems</returns>
		public static IList<string> GetNames()
		{
			lock(s_SyncRoot)
			{
				return s_FileSystems.Keys.ToArray();
			}
		}

		/// <summary>
		/// Removes a registered file system
		/// </summary>
		/// <param name="filesystemName">The name of the file system to remove</param>
		/// <returns>true if the file system was removed, otherwise false</returns>
		public static bool Remove(string filesystemName)
		{
			VirtualFileSystem.ValidateName(filesystemName);

			lock(s_SyncRoot)
			{
				return s_FileSystems.Remove(filesystemName);
			}
		}

		/// <summary>
		/// Removes all file systems
		/// </summary>
		public static void Clear()
		{
			lock(s_SyncRoot)
			{
				s_FileSystems.Clear();
			}
		}
	}
}
