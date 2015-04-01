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
	/// An implementation of a directory node
	/// </summary>
	public class DirectoryNode : IDirectoryNode
	{
		private readonly object m_SyncRoot=new object();
		private readonly Dictionary<string,INode> m_Contents=new Dictionary<string,INode>(IgnoreCaseEqualityComparer.Instance);

		/// <summary>
		/// The lock object to use
		/// </summary>
		protected object SyncRoot
		{
			get{return m_SyncRoot;}
		}

		/// <summary>
		/// Always returns false
		/// </summary>
		public bool IsFile
		{
			get{return false;}
		}

		/// <summary>
		/// Always returns true
		/// </summary>
		public bool IsDirectory
		{
			get{return true;}
		}

		/// <summary>
		/// Returns the files in the directory
		/// </summary>
		/// <returns>A list of files</returns>
		public virtual IList<string> GetFiles()
		{
			var files=new List<string>();

			lock(m_SyncRoot)
			{
				foreach(var pair in m_Contents)
				{
					if(pair.Value.IsFile) files.Add(pair.Key);
				}
			}

			return files;
		}

		/// <summary>
		/// Creates a directory (if it doesn't already exist)
		/// </summary>
		/// <param name="name">The name of the directory to create</param>
		/// <returns>On success the directory node representing the directory, otherwise false</returns>
		public virtual IDirectoryNode CreateDirectory(string name)
		{
			if(name==null) throw new ArgumentNullException("name");
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name cannot be whitespace","name");

			lock(m_SyncRoot)
			{
				INode node;
				if(m_Contents.TryGetValue(name,out node))
				{
					if(node.IsDirectory)
					{
						return (IDirectoryNode)node;
					}
					else
					{
						return null;
					}
				}
				else
				{
					IDirectoryNode directory=new DirectoryNode();
					m_Contents.Add(name,directory);

					return directory;
				}
			}
		}

		/// <summary>
		/// Creates or replaces a file in the directory
		/// </summary>
		/// <param name="name">The name of the file</param>
		/// <param name="file">The file</param>
		/// <returns>On success a file node for the file, otherwise null</returns>
		public virtual IFileNode CreateFile(string name, Func<Stream> file)
		{
			if(name==null) throw new ArgumentNullException("name");
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name cannot be whitespace","name");
			if(file==null) throw new ArgumentNullException("file");

			lock(m_SyncRoot)
			{
				INode node;
				if(m_Contents.TryGetValue(name,out node))
				{
					if(node.IsFile)
					{
						// We can replace a file with a new one
						var fileNode=new FileNode(file);
						m_Contents[name]=fileNode;

						return fileNode;
					}
					else
					{
						return null;
					}
				}
				else
				{
					var fileNode=new FileNode(file);
					m_Contents.Add(name,fileNode);

					return fileNode;
				}
			}
		}

		/// <summary>
		/// Returns a list of directories within the directory.
		/// Note, this also includes mount points
		/// </summary>
		/// <returns>A list of directories</returns>
		public virtual IList<string> GetDirectories()
		{
			var directories=new List<string>();

			lock(m_SyncRoot)
			{
				foreach(var pair in m_Contents)
				{
					if(pair.Value.IsDirectory) directories.Add(pair.Key);
				}
			}

			return directories;
		}

		/// <summary>
		/// Opens the file with the specified name
		/// </summary>
		/// <param name="name">The name of the file to open</param>
		/// <returns>On success a stream to the file, otherwise null</returns>
		public virtual Stream OpenFile(string name)
		{
			if(name==null) throw new ArgumentNullException("name");
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name cannot be whitespace","name");

			INode node=null;
			bool foundFile=false;

			lock(m_SyncRoot)
			{
				foundFile=m_Contents.TryGetValue(name,out node);
			}

			if(foundFile)
			{
				if(node.IsFile)
				{
					return ((IFileNode)node).Open();
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Attempts to get the directory with the specified name
		/// </summary>
		/// <param name="name">The name of the directory to get</param>
		/// <param name="directory">On success the directory node representing the name, otherwise null</param>
		/// <returns>The success of the operation</returns>
		public virtual LookupResult TryGetDirectory(string name, out IDirectoryNode directory)
		{
			if(name==null) throw new ArgumentNullException("name");
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name cannot be whitespace","name");

			INode node=null;
			bool foundDirectory=false;

			lock(m_SyncRoot)
			{
				foundDirectory=m_Contents.TryGetValue(name,out node);
			}

			if(foundDirectory)
			{
				directory=node as IDirectoryNode;
				return directory!=null ? LookupResult.Success : LookupResult.WrongType;
			}
			else
			{
				directory=null;
				return LookupResult.NotFound;
			}
		}

		/// <summary>
		/// Attempts to get the file with the specified name
		/// </summary>
		/// <param name="name">The name of the file to get</param>
		/// <param name="file">On success the file node representing the name, otherwise null</param>
		/// <returns>The success of the operation</returns>
		public virtual LookupResult TryGetFile(string name, out IFileNode file)
		{
			if(name==null) throw new ArgumentNullException("name");
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name cannot be whitespace","name");

			bool foundFile=false;
			INode node=null;

			lock(m_SyncRoot)
			{
				foundFile=m_Contents.TryGetValue(name,out node);
			}

			if(foundFile)
			{
				file=node as IFileNode;
				return file!=null ? LookupResult.Success : LookupResult.WrongType;
			}
			else
			{
				file=null;
				return LookupResult.NotFound;
			}
		}

		/// <summary>
		/// Attempts to delete the item with the specified name
		/// </summary>
		/// <param name="name">The name of the item to delete</param>
		/// <returns>true if the item was deleted, false otherwise</returns>
		public virtual bool Delete(string name)
		{
			if(name==null) throw new ArgumentNullException("name");
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name cannot be whitespace","name");

			lock(m_SyncRoot)
			{
				return m_Contents.Remove(name);
			}
		}

		/// <summary>
		/// Registers a mount point in a directory
		/// </summary>
		/// <param name="name">The name of the mount point</param>
		/// <param name="mountPoint">The mount point to register</param>
		/// <returns>true if the mount point was registered, otherwise false</returns>
		public virtual bool RegisterMount(string name, IMountPointNode mountPoint)
		{
			if(name==null) throw new ArgumentNullException("name");
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name cannot be whitespace","name");
			if(mountPoint==null) throw new ArgumentNullException("mountPoint");

			if(mountPoint.IsDirectory==false) throw new IOException("mount points must be directories");

			lock(m_SyncRoot)
			{
				if(m_Contents.ContainsKey(name))
				{
					return false;
				}
				else
				{
					m_Contents.Add(name,mountPoint);
					return true;
				}
			}
		}

		/// <summary>
		/// Attempts to get a mount point with the specified name
		/// </summary>
		/// <param name="name">The name of the mount point</param>
		/// <param name="mountPoint">On success the mount point node, otherwise null</param>
		/// <returns>The success of the operation</returns>
		public virtual LookupResult TryGetMountPoint(string name, out IMountPointNode mountPoint)
		{
			if(name==null) throw new ArgumentNullException("name");
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name cannot be whitespace","name");

			INode node;
			bool found=false;

			lock(m_SyncRoot)
			{
				found=m_Contents.TryGetValue(name,out node);
			}

			if(found)
			{
				if(node.IsDirectory)
				{
					mountPoint=node as IMountPointNode;
					return mountPoint!=null ? LookupResult.Success : LookupResult.WrongType;
				}
				else
				{
					mountPoint=null;
					return LookupResult.WrongType;
				}
			}
			else
			{
				mountPoint=null;
				return LookupResult.NotFound;
			}
		}
	}
}
