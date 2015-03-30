using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Storage.Vfs
{
	/// <summary>
	/// Useful helper functions for file spaces
	/// </summary>
	public static class FilespaceExtensions
	{
		public static void CreateDirectory(this Filespace filespace, string unixPath)
		{
			var path=Filespace.FromUnixPath(unixPath);
			filespace.CreateDirectory(path);
		}

		public static IList<string> GetDirectories(this Filespace filespace, string unixPath)
		{
			var path=Filespace.FromUnixPath(unixPath);
			return filespace.GetDirectories(path);
		}

		public static bool DirectoryExists(this Filespace filespace, string unixPath)
		{
			var path=Filespace.FromUnixPath(unixPath);
			return filespace.DirectoryExists(path);
		}

		public static void CreateFile(this Filespace filespace, string unixPath, Func<Stream> file)
		{
			var path=Filespace.FromUnixPath(unixPath);
			filespace.CreateFile(path,file);
		}

		public static void CreateFile(this Filespace filespace, string unixPath, string fileText)
		{
			var path=Filespace.FromUnixPath(unixPath);
			var file=Filespace.CreateTextFile(fileText);

			filespace.CreateFile(path,file);
		}

		public static void CreateFile(this Filespace filespace, string unixPath, byte[] binaryData)
		{
			var path=Filespace.FromUnixPath(unixPath);
			var file=Filespace.CreateBinaryFile(binaryData);

			filespace.CreateFile(path,file);
		}

		public static IList<string> GetFiles(this Filespace filespace, string unixPath)
		{
			var path=Filespace.FromUnixPath(unixPath);
			return filespace.GetFiles(path);
		}

		public static Stream OpenFile(this Filespace filespace, string unixPath)
		{
			var path=Filespace.FromUnixPath(unixPath);
			return filespace.OpenFile(path);
		}

		public static bool FileExists(this Filespace filespace, string unixPath)
		{
			var path=Filespace.FromUnixPath(unixPath);
			return filespace.FileExists(path);
		}
	}
}
