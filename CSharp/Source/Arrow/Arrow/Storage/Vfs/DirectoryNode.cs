using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Collections;

namespace Arrow.Storage.Vfs
{
	class DirectoryNode : IDirectoryNode
	{
		private readonly object m_SyncRoot=new object();
		private readonly Dictionary<string,INode> m_Contents=new Dictionary<string,INode>(IgnoreCaseEqualityComparer.Instance);

		public bool IsFile
		{
			get{return false;}
		}

		public bool IsDirectory
		{
			get{return true;}
		}

		public IList<string> GetFiles()
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

		public IDirectoryNode CreateDirectory(string name)
		{
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
						throw new IOException("item already exists and is not a directory: "+name);
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

		public IFileNode CreateFile(string name, Func<Stream> file)
		{
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
						throw new IOException("item already exists and is not a file: "+name);
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

		public IList<string> GetDirectories()
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

		public Stream OpenFile(string name)
		{
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
					throw new IOException("not a file: "+name);
				}
			}
			else
			{
				throw new IOException("file not found: "+name);
			}
		}

		public IDirectoryNode GetDirectory(string name)
		{
			bool foundDirectory=false;
			INode node=null;

			lock(m_SyncRoot)
			{
				foundDirectory=m_Contents.TryGetValue(name,out node);
			}

			if(foundDirectory)
			{
				if(node.IsDirectory)
				{
					return (IDirectoryNode)node;
				}
				else
				{
					throw new IOException("not a directory: "+name);
				}
			}
			else
			{
				throw new IOException("directory not found: "+name);
			}
		}

		public bool TryGetDirectory(string name, out IDirectoryNode directory)
		{
			INode node=null;
			bool foundDirectory=false;

			lock(m_SyncRoot)
			{
				foundDirectory=m_Contents.TryGetValue(name,out node);
			}

			if(foundDirectory)
			{
				directory=node as IDirectoryNode;
				return directory!=null;
			}
			else
			{
				directory=null;
				return false;
			}
		}

		public bool TryGetFile(string name, out IFileNode file)
		{
			bool foundFile=false;
			INode node=null;

			lock(m_SyncRoot)
			{
				foundFile=m_Contents.TryGetValue(name,out node);
			}

			if(foundFile)
			{
				file=node as IFileNode;
				return file!=null;
			}
			else
			{
				file=null;
				return false;
			}
		}

		public bool Delete(string name)
		{
			lock(m_SyncRoot)
			{
				return m_Contents.Remove(name);
			}
		}
	}
}
