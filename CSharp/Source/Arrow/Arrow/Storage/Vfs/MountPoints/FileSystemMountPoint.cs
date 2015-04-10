using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Storage.Vfs.MountPoints
{
	/// <summary>
	/// A mount point that redirects to a directory in the file system.
	/// The file system is mounted as read only
	/// </summary>
	public class FileSystemMountPoint : DirectoryNode
	{
		private readonly string m_RootDirectory;

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="rootDirectory">The root directory to redirec to</param>
		public FileSystemMountPoint(string rootDirectory)
		{
			if(rootDirectory==null) throw new ArgumentNullException("rootDirectory");
			if(Directory.Exists(rootDirectory)==false) throw new IOException("root directory does not exist");

			m_RootDirectory=rootDirectory;
		}

		/// <summary>
		/// Returns the names of files within the directory
		/// </summary>
		/// <returns>a list of filenames</returns>
		public override IList<string> GetFiles()
		{
			var fullyQualifiedFiles=Directory.GetFiles(m_RootDirectory);

			var files=new List<string>(fullyQualifiedFiles.Length);
			foreach(var path in fullyQualifiedFiles)
			{
				string filename=Path.GetFileName(path);
				files.Add(filename);
			}

			return files;
		}

		/// <summary>
		/// Returns the names of directories within the directory
		/// </summary>
		/// <returns></returns>
		public override IList<string> GetDirectories()
		{
			var fullyQualifiedFiles=Directory.GetDirectories(m_RootDirectory);

			var files=new List<string>(fullyQualifiedFiles.Length);
			foreach(var path in fullyQualifiedFiles)
			{
				string filename=Path.GetFileName(path);
				files.Add(filename);
			}

			return files;
		}

		/// <summary>
		/// Throws an IOException and directories are read only
		/// </summary>
		/// <param name="name">The name of the directory to create</param>
		/// <returns>A directory node representing the directory, or null if the directory could not created</returns>
		public override DirectoryNode CreateDirectory(string name)
		{
			throw new IOException("directory is read-only: "+m_RootDirectory);
		}

		/// <summary>
		/// Throws an IOException and directories are read only
		/// </summary>
		/// <param name="name">The name of the file</param>
		/// <param name="file">A function that returns the contents of the file</param>
		/// <returns>The node for the file if successful, otherwise null</returns>
		public override FileNode CreateFile(string name, Func<Stream> file)
		{
			throw new IOException("directory is read-only: "+m_RootDirectory);
		}

		/// <summary>
		/// Attempts to get the directory with the specified name
		/// </summary>
		/// <param name="name">The name of the directory to get</param>
		/// <param name="directory">On success the directory node representing the name, otherwise null</param>
		/// <returns>The success of the operation</returns>
		public override LookupResult TryGetDirectory(string name, out DirectoryNode directory)
		{
			ValidateName(name);

			string fullPath=Path.Combine(m_RootDirectory,name);
			
			if(Directory.Exists(fullPath))
			{
				directory=new FileSystemMountPoint(fullPath);
				return LookupResult.Success;
			}
			
			directory=null;
			return LookupResult.NotFound;
		}

		/// <summary>
		/// Attempts to get the file with the specified name
		/// </summary>
		/// <param name="name">The name of the file to get</param>
		/// <param name="file">On success the file node representing the name, otherwise null</param>
		/// <returns>The success of the operation</returns>
		public override LookupResult TryGetFile(string name, out FileNode file)
		{
			ValidateName(name);

			string filename=Path.Combine(m_RootDirectory,name);

			if(File.Exists(filename))
			{
				file=new FileSystemFileNode(filename);
				return LookupResult.Success;
			}
			else
			{
				file=null;
				return LookupResult.NotFound;
			}
		}

		/// <summary>
		/// Throws an IOException and directories are read only
		/// </summary>
		/// <param name="name">The name of the item to delete</param>
		/// <returns>true if the item was deleted, false otherwise</returns>
		public override bool Delete(string name)
		{
			throw new IOException("directory is read-only: "+m_RootDirectory);
		}

		/// <summary>
		/// Throws a NotImplementedException as file system mounts do not support mount points
		/// </summary>
		/// <param name="name">The name of the mount point</param>
		/// <param name="mountPoint">The mount point to register</param>
		/// <returns>true if the mount point was registered, otherwise false</returns>
		public override bool RegisterMount(string name, DirectoryNode mountPoint)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Throws a NotImplementedException as file system mounts do not support mount points
		/// </summary>
		/// <param name="name">The name of the mount point</param>
		/// <param name="mountPoint">On success the mount point node, otherwise null</param>
		/// <returns>The success of the operation</returns>
		public override LookupResult TryGetMountPoint(string name, out DirectoryNode mountPoint)
		{
			throw new NotImplementedException();
		}

		class FileSystemFileNode : FileNode
		{
			private readonly string m_Filename;

			public FileSystemFileNode(string filename)
			{
				m_Filename=filename;
			}

			public override Stream Open()
			{
				return File.OpenRead(m_Filename);
			}
		}
	}
}
