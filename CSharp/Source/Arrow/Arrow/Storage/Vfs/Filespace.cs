using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Storage.Vfs
{
	/// <summary>
	/// An in-memory filesystem.
	/// This class is thread safe.
	/// </summary>
	public class Filespace
	{
		private static readonly char[] UnixSeperator=new char[]{'/'};
		private static readonly string[] Empty=new string[0];

		private readonly IDirectoryNode m_Root=new DirectoryNode();

		public static readonly IReadOnlyList<string> Root=new List<string>();

		/// <summary>
		/// Creates a directory.
		/// If any parts of the path do not exist they are created as part of the process
		/// </summary>
		/// <param name="path">The path to the directory</param>
		public void CreateDirectory(IReadOnlyList<string> path)
		{
			if(path==null) throw new ArgumentNullException("path");
			if(path.Count==0) throw new ArgumentException("path is empty","path");

			IDirectoryNode node=m_Root;

			foreach(string name in path)
			{
				node=node.CreateDirectory(name);
			}
		}

		/// <summary>
		/// Returns a list of directories at the specified path
		/// </summary>
		/// <param name="path">The path to get directories from</param>
		/// <returns>A list of directories at the path, or an empty list if non exist</returns>
		public IList<string> GetDirectories(IReadOnlyList<string> path)
		{
			if(path==null) throw new ArgumentNullException("path");

			IDirectoryNode node=m_Root;

			foreach(string name in path)
			{
				if(node.TryGetDirectory(name,out node)==false)
				{
					return Empty;
				}
			}

			return node.GetDirectories();
		}

		/// <summary>
		/// Checks to see if a directory exists
		/// </summary>
		/// <param name="path">The path to the directory</param>
		/// <returns>true if a directory with the given name exists, otherwise false</returns>
		public bool DirectoryExists(IReadOnlyList<string> path)
		{
			if(path==null) throw new ArgumentNullException("path");

			IDirectoryNode node=m_Root;

			for(int i=0; i<path.Count && node!=null; i++)
			{
				string name=path[i];

				node.TryGetDirectory(name,out node);
			}

			return node!=null;
		}

		/// <summary>
		/// Creates a file at the specified path.
		/// If the file already exists it is overwritten.
		/// </summary>
		/// <param name="path">The path of the file. The last part is the filename</param>
		/// <param name="file">A function that returns the file data</param>
		public void CreateFile(IReadOnlyList<string> path, Func<Stream> file)
		{
			if(path==null) throw new ArgumentNullException("path");
			if(file==null) throw new ArgumentNullException("file");

			string filename=path[path.Count-1];

			IDirectoryNode node=m_Root;
			for(int i=0; i<path.Count-1; i++)
			{
				string name=path[i];
				node=node.CreateDirectory(name);
			}

			node.CreateFile(filename,file);
		}

		/// <summary>
		/// Returns a list of files at the specified path
		/// </summary>
		/// <param name="path">The path to get files from</param>
		/// <returns>A list of files at the path, or an empty list if non exist</returns>
		public IList<string> GetFiles(IReadOnlyList<string> path)
		{
			if(path==null) throw new ArgumentNullException("path");

			IDirectoryNode node=m_Root;

			foreach(string name in path)
			{
				if(node.TryGetDirectory(name,out node)==false)
				{
					return Empty;
				}
			}

			return node.GetFiles();
		}

		/// <summary>
		/// Opens the file at the specified path
		/// </summary>
		/// <param name="path">The path to the file</param>
		/// <returns>A stream to the file at the path</returns>
		public Stream OpenFile(IReadOnlyList<string> path)
		{
			if(path==null) throw new ArgumentNullException("path");
			if(path.Count==0) throw new ArgumentException("path is empty","path");

			string filename=path[path.Count-1];

			IDirectoryNode node=DirectoryForFile(path);
			if(node==null) throw new IOException("file not found");

			return node.OpenFile(filename);
		}

		/// <summary>
		/// Checks to see if a directory exists
		/// </summary>
		/// <param name="path">The path to the directory</param>
		/// <returns>true if a directory with the given name exists, otherwise false</returns>
		public bool FileExists(IReadOnlyList<string> path)
		{
			if(path==null) throw new ArgumentNullException("path");
			if(path.Count==0) throw new ArgumentException("path is empty","path");

			string filename=path[path.Count-1];
			IDirectoryNode node=DirectoryForFile(path);

			IFileNode fileNode=null;
			return node!=null && node.TryGetFile(filename,out fileNode);
		}

		private IDirectoryNode DirectoryForFile(IReadOnlyList<string> path)
		{
			IDirectoryNode node=m_Root;

			// The last part of the path is the filename, so ignore it
			for(int i=0; i<path.Count-1 && node!=null; i++)
			{
				string name=path[i];

				node.TryGetDirectory(name,out node);
			}

			return node;
		}

		/// <summary>
		/// Creates a path list
		/// </summary>
		/// <param name="parts">The parts that make up the path</param>
		/// <returns>A path list</returns>
		public static IReadOnlyList<string> MakePath(params string[] parts)
		{
			if(parts.Length==0) return Root;

			List<string> pathParts=new List<string>(parts.Length);

			foreach(var part in parts)
			{
				if(part==null) throw new ArgumentException("part of path is null","parts");

				string normalizedPart=part.Trim();
				if(string.IsNullOrEmpty(normalizedPart)) throw new ArgumentException("part of path is invalid (empty of whitespace)","parts");

				pathParts.Add(normalizedPart);
			}

			return pathParts;
		}
		
		/// <summary>
		/// Creates a path list from a Unix style path (eg /path/to/file)
		/// </summary>
		/// <param name="path">The unix path</param>
		/// <returns>A list representing the path</returns>
		public static IReadOnlyList<string> FromUnixPath(string path)
		{
			if(path==null) throw new ArgumentNullException("path");

			string[] parts=path.Split(UnixSeperator,StringSplitOptions.RemoveEmptyEntries);
			if(parts.Length==0) return Root;

			return new List<string>(parts);
		}

		
		/// <summary>
		/// Creates a text file function. The text is converted to UTF8 encoding
		/// </summary>
		/// <param name="text">The text of the file</param>
		/// <returns>A function that returns a stream to the text</returns>
		public static Func<Stream> CreateTextFile(string text)
		{
			return CreateTextFile(text,Encoding.UTF8);
		}

		/// <summary>
		/// Creates a text file function
		/// </summary>
		/// <param name="text">The text of the file</param>
		/// <param name="encoding">The encoding to use when converting the text</param>
		/// <returns>A function that returns a stream to the text</returns>
		public static Func<Stream> CreateTextFile(string text, Encoding encoding)
		{
			if(text==null) throw new ArgumentNullException("text");
			if(encoding==null) throw new ArgumentNullException("encoding");

			return ()=>
			{
				var bytes=encoding.GetBytes(text);
				return new MemoryStream(bytes,false);
			};
		}

		/// <summary>
		/// Create a binary file,
		/// Note that the returned function holds a reference to the supplied data
		/// and and change to this will affect the data returned by the buffer
		/// </summary>
		/// <param name="data">The data for the file</param>
		/// <returns>A function that returns a stream to the binary data</returns>
		public static Func<Stream> CreateBinaryFile(byte[] data)
		{
			if(data==null) throw new ArgumentNullException("data");

			return ()=>
			{
				return new MemoryStream(data,false);
			};
		}
	}
}
