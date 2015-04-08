using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Storage.Vfs
{
	/// <summary>
	/// Base interface for directories within the virtual file system
	/// </summary>
	public abstract class DirectoryNode : INode
	{
		/// <summary>
		/// Returns the names of files within the directory
		/// </summary>
		/// <returns>a list of filenames</returns>
		public abstract IList<string> GetFiles();

		/// <summary>
		/// Returns the names of directories within the directory
		/// </summary>
		/// <returns></returns>
		public abstract IList<string> GetDirectories();

		/// <summary>
		/// Attempts to create a directory with the specified name
		/// </summary>
		/// <param name="name">The name of the directory to create</param>
		/// <returns>A directory node representing the directory, or null if the directory could not created</returns>
		public abstract DirectoryNode CreateDirectory(string name);

		/// <summary>
		/// Create a file
		/// </summary>
		/// <param name="name">The name of the file</param>
		/// <param name="file">A function that returns the contents of the file</param>
		/// <returns>The node for the file if successful, otherwise null</returns>
		public abstract FileNode CreateFile(string name, Func<Stream> file);

		/// <summary>
		/// Attempts to get the directory with the specified name
		/// </summary>
		/// <param name="name">The name of the directory to get</param>
		/// <param name="directory">On success the directory node representing the name, otherwise null</param>
		/// <returns>The success of the operation</returns>
		public abstract LookupResult TryGetDirectory(string name, out DirectoryNode directory);
		
		/// <summary>
		/// Attempts to get the file with the specified name
		/// </summary>
		/// <param name="name">The name of the file to get</param>
		/// <param name="file">On success the file node representing the name, otherwise null</param>
		/// <returns>The success of the operation</returns>
		public abstract LookupResult TryGetFile(string name, out FileNode file);

		/// <summary>
		/// Attempts to delete the item with the specified name
		/// </summary>
		/// <param name="name">The name of the item to delete</param>
		/// <returns>true if the item was deleted, false otherwise</returns>
		public abstract bool Delete(string name);

		/// <summary>
		/// Registers a mount point in a directory
		/// </summary>
		/// <param name="name">The name of the mount point</param>
		/// <param name="mountPoint">The mount point to register</param>
		/// <returns>true if the mount point was registered, otherwise false</returns>
		public abstract bool RegisterMount(string name, MountPointNode mountPoint);

		/// <summary>
		/// Attempts to get a mount point with the specified name
		/// </summary>
		/// <param name="name">The name of the mount point</param>
		/// <param name="mountPoint">On success the mount point node, otherwise null</param>
		/// <returns>The success of the operation</returns>
		public abstract LookupResult TryGetMountPoint(string name, out MountPointNode mountPoint);

		/// <summary>
		/// Checks that a name is valid and throws an exception if not
		/// </summary>
		/// <param name="name">The name to validate</param>
		protected void ValidateName(string name)
		{
			if(name==null) throw new ArgumentNullException("name");
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name is whitespace","name");

			if(name.Contains('/')) throw new ArgumentException("name cannot contain /","name");

			var trimmed=name.Trim();
			if(trimmed=="." || trimmed=="..") throw new ArgumentException("name cannot be . or ..",name);
		}


		bool INode.IsFile
		{
			get{return false;}
		}

		bool INode.IsDirectory
		{
			get{return true;}
		}

	}
}
