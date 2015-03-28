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
		private readonly IDirectoryNode m_Root=new DirectoryNode();

		/// <summary>
		/// Creates a directory.
		/// If any parts of the path do not exist they are created as part of the process
		/// </summary>
		/// <param name="path">The path to the directory</param>
		public void CreateDirectory(IList<string> path)
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
		public IList<string> GetDirectories(IList<string> path)
		{
			if(path==null) throw new ArgumentNullException("path");
			if(path.Count==0) throw new ArgumentException("path is empty","path");

			IDirectoryNode node=m_Root;

			foreach(string name in path)
			{
				node=node.GetDirectory(name);
			}

			return node.GetDirectories();
		}

		/// <summary>
		/// Creates a file at the specified path.
		/// If the file already exists it is overwritten.
		/// </summary>
		/// <param name="path">The path of the file. The last part is the filename</param>
		/// <param name="file">A function that returns the file data</param>
		public void CreateFile(IList<string> path, Func<Stream> file)
		{
			if(path==null) throw new ArgumentNullException("path");
			if(path.Count==0) throw new ArgumentException("path is empty","path");
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
		public IList<string> GetFiles(IList<string> path)
		{
			if(path==null) throw new ArgumentNullException("path");
			if(path.Count==0) throw new ArgumentException("path is empty","path");

			IDirectoryNode node=m_Root;

			foreach(string name in path)
			{
				node=node.GetDirectory(name);
			}

			return node.GetFiles();
		}

		/// <summary>
		/// Opens the file at the specified path
		/// </summary>
		/// <param name="path">The path to the file</param>
		/// <returns>A stream to the file at the path</returns>
		public Stream OpenFile(IList<string> path)
		{
			if(path==null) throw new ArgumentNullException("path");
			if(path.Count==0) throw new ArgumentException("path is empty","path");

			string filename=path[path.Count-1];

			IDirectoryNode node=m_Root;
			for(int i=0; i<path.Count-1; i++)
			{
				string name=path[i];
				node=node.CreateDirectory(name);
			}

			return node.OpenFile(filename);
		}

		/// <summary>
		/// Creates a path list
		/// </summary>
		/// <param name="parts">The parts that make up the path</param>
		/// <returns>A path list</returns>
		public static IList<string> MakePath(params string[] parts)
		{
			if(parts.Length==0) throw new ArgumentException("not enough path parts","parts");

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
		/// Create a binary file
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
