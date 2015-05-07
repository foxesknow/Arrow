﻿using System;
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
	public partial class DefaultDirectoryNode : IDirectoryNode
	{
		private readonly object m_SyncRoot=new object();
		private readonly Dictionary<string,INode> m_Contents=new Dictionary<string,INode>(IgnoreCaseEqualityComparer.Instance);

		/// <summary>
		/// The lock object to use
		/// </summary>
		public object SyncRoot
		{
			get{return m_SyncRoot;}
		}

		/// <summary>
		/// Returns the files in the directory
		/// </summary>
		/// <returns>A list of files</returns>
		public IList<string> GetFiles()
		{
			var files=new List<string>();

			lock(m_SyncRoot)
			{
				foreach(var pair in m_Contents)
				{
					if(pair.Value is IFileNode) files.Add(pair.Key);
				}
			}

			return files;
		}

		/// <summary>
		/// Creates a directory (if it doesn't already exist)
		/// </summary>
		/// <param name="name">The name of the directory to create</param>
		/// <returns>On success the directory node representing the directory, otherwise false</returns>
		public IDirectoryNode CreateDirectory(string name)
		{
			VirtualFileSystem.ValidateName(name);

			lock(m_SyncRoot)
			{
				INode node;
				if(m_Contents.TryGetValue(name,out node))
				{
					return node as IDirectoryNode;
				}
				else
				{
					IDirectoryNode directory=new DefaultDirectoryNode();
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
		public IFileNode CreateFile(string name, Func<Stream> file)
		{
			VirtualFileSystem.ValidateName(name);
			if(file==null) throw new ArgumentNullException("file");

			lock(m_SyncRoot)
			{
				INode node;
				if(m_Contents.TryGetValue(name,out node))
				{
					if(node is IFileNode)
					{
						// We can replace a file with a new one
						var fileNode=new DefaultFileNode(file);
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
					var fileNode=new DefaultFileNode(file);
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
		public IList<string> GetDirectories()
		{
			var directories=new List<string>();

			lock(m_SyncRoot)
			{
				foreach(var pair in m_Contents)
				{
					if(pair.Value is IDirectoryNode) directories.Add(pair.Key);
				}
			}

			return directories;
		}

		/// <summary>
		/// Attempts to get the directory with the specified name
		/// </summary>
		/// <param name="name">The name of the directory to get</param>
		/// <param name="directory">On success the directory node representing the name, otherwise null</param>
		/// <returns>The success of the operation</returns>
		public LookupResult TryGetDirectory(string name, out IDirectoryNode directory)
		{
			VirtualFileSystem.ValidateName(name);

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
		public LookupResult TryGetFile(string name, out IFileNode file)
		{
			VirtualFileSystem.ValidateName(name);

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
		public bool Delete(string name)
		{
			VirtualFileSystem.ValidateName(name);

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
		public bool RegisterMount(string name, IDirectoryNode mountPoint)
		{
			VirtualFileSystem.ValidateName(name);
			if(mountPoint==null) throw new ArgumentNullException("mountPoint");

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
		public LookupResult TryGetMountPoint(string name, out IDirectoryNode mountPoint)
		{
			VirtualFileSystem.ValidateName(name);

			INode node;
			bool found=false;

			lock(m_SyncRoot)
			{
				found=m_Contents.TryGetValue(name,out node);
			}

			if(found)
			{
				mountPoint=node as IDirectoryNode;

				if(mountPoint!=null)
				{
					return LookupResult.Success;
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