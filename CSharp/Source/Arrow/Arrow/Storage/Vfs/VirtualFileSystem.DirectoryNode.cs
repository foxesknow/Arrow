using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Storage.Vfs
{
	public partial class VirtualFileSystem : DirectoryNode
	{
		/// <summary>
		/// Returns the names of files in the root
		/// </summary>
		/// <returns>a list of filenames</returns>
		public override IList<string> GetFiles()
		{
			return m_Root.GetFiles();
		}

		/// <summary>
		/// Returns the names of directories in the root
		/// </summary>
		/// <returns>a list of directories</returns>
		public override IList<string> GetDirectories()
		{
			return m_Root.GetDirectories();
		}

		/// <summary>
		/// Attempts to create a directory with the specified name
		/// </summary>
		/// <param name="name">The name of the directory to create</param>
		/// <returns>A directory node representing the directory, or null if the directory could not created</returns>
		public override DirectoryNode CreateDirectory(string name)
		{
			return m_Root.CreateDirectory(name);
		}

		/// <summary>
		/// Create a file
		/// </summary>
		/// <param name="name">The name of the file</param>
		/// <param name="file">A function that returns the contents of the file</param>
		/// <returns>The node for the file if successful, otherwise null</returns>
		public override FileNode CreateFile(string name, Func<System.IO.Stream> file)
		{
			return m_Root.CreateFile(name,file);
		}

		/// <summary>
		/// Attempts to get the directory with the specified name
		/// </summary>
		/// <param name="name">The name of the directory to get</param>
		/// <param name="directory">On success the directory node representing the name, otherwise null</param>
		/// <returns>The success of the operation</returns>
		public override LookupResult TryGetDirectory(string name, out DirectoryNode directory)
		{
			return m_Root.TryGetDirectory(name,out directory);
		}

		/// <summary>
		/// Attempts to get the file with the specified name
		/// </summary>
		/// <param name="name">The name of the file to get</param>
		/// <param name="file">On success the file node representing the name, otherwise null</param>
		/// <returns>The success of the operation</returns>
		public override LookupResult TryGetFile(string name, out FileNode file)
		{
			return m_Root.TryGetFile(name,out file);
		}

		/// <summary>
		/// Attempts to delete the item with the specified name
		/// </summary>
		/// <param name="name">The name of the item to delete</param>
		/// <returns>true if the item was deleted, false otherwise</returns>
		public override bool Delete(string name)
		{
			return m_Root.Delete(name);
		}

		/// <summary>
		/// Registers a mount point in the root
		/// </summary>
		/// <param name="name">The name of the mount point</param>
		/// <param name="mountPoint">The mount point to register</param>
		/// <returns>true if the mount point was registered, otherwise false</returns>
		public override bool RegisterMount(string name, DirectoryNode mountPoint)
		{
			return m_Root.RegisterMount(name,mountPoint);
		}

		/// <summary>
		/// Attempts to get a mount point with the specified name
		/// </summary>
		/// <param name="name">The name of the mount point</param>
		/// <param name="mountPoint">On success the mount point node, otherwise null</param>
		/// <returns>The success of the operation</returns>
		public override LookupResult TryGetMountPoint(string name, out DirectoryNode mountPoint)
		{
			return m_Root.TryGetMountPoint(name,out mountPoint);
		}

		private IReadOnlyList<string> ToList(string name)
		{
			return new string[]{name};
		}
	}
}
