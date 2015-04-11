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
	public static class VirtualFileSystemExtensions
	{
		/// <summary>
		/// Creates a directory
		/// </summary>
		/// <param name="filespace">The filesystem</param>
		/// <param name="unixPath">The unix path of for the directory</param>
		public static void CreateDirectory(this VirtualFileSystem filespace, string unixPath)
		{
			var path=VirtualFileSystem.FromUnixPath(unixPath);
			filespace.CreateDirectory(path);
		}

		/// <summary>
		/// Get all the directories in the specified directory
		/// </summary>
		/// <param name="filespace">The filesystem</param>
		/// <param name="unixPath">The unix path to get the directories from</param>
		/// <returns>A list of directories</returns>
		public static IList<string> GetDirectories(this VirtualFileSystem filespace, string unixPath)
		{
			var path=VirtualFileSystem.FromUnixPath(unixPath);
			return filespace.GetDirectories(path);
		}

		/// <summary>
		/// Checks to see if a directory exists
		/// </summary>
		/// <param name="filespace">The filesystem</param>
		/// <param name="unixPath">The unix path of the directory to check for</param>
		/// <returns>true if the directory exists, otherwise false</returns>
		public static bool DirectoryExists(this VirtualFileSystem filespace, string unixPath)
		{
			var path=VirtualFileSystem.FromUnixPath(unixPath);
			return filespace.DirectoryExists(path);
		}

		/// <summary>
		/// Creates a file
		/// </summary>
		/// <param name="filespace">The filesystem</param>
		/// <param name="unixPath">The unix path of the file to create</param>
		/// <param name="file">The file</param>
		public static void CreateFile(this VirtualFileSystem filespace, string unixPath, Func<Stream> file)
		{
			var path=VirtualFileSystem.FromUnixPath(unixPath);
			filespace.CreateFile(path,file);
		}

		/// <summary>
		/// Creates a file
		/// </summary>
		/// <param name="filespace">The filesystem</param>
		/// <param name="unixPath">The unix path of the file to create</param>
		/// <param name="fileText">The text of the file to create</param>
		public static void CreateFile(this VirtualFileSystem filespace, string unixPath, string fileText)
		{
			var path=VirtualFileSystem.FromUnixPath(unixPath);
			var file=VirtualFileSystem.CreateTextFile(fileText);

			filespace.CreateFile(path,file);
		}

		/// <summary>
		/// Creates a file
		/// </summary>
		/// <param name="filespace">The filesystem</param>
		/// <param name="unixPath">The unix path of the file to create</param>
		/// <param name="binaryData">The binary data of the file</param>
		public static void CreateFile(this VirtualFileSystem filespace, string unixPath, byte[] binaryData)
		{
			var path=VirtualFileSystem.FromUnixPath(unixPath);
			var file=VirtualFileSystem.CreateBinaryFile(binaryData);

			filespace.CreateFile(path,file);
		}

		/// <summary>
		/// Gets all the files in a directory
		/// </summary>
		/// <param name="filespace">The filesystem</param>
		/// <param name="unixPath">The path of the directory to get the files from</param>
		/// <returns>A list of files</returns>
		public static IList<string> GetFiles(this VirtualFileSystem filespace, string unixPath)
		{
			var path=VirtualFileSystem.FromUnixPath(unixPath);
			return filespace.GetFiles(path);
		}

		/// <summary>
		/// Opens a file
		/// </summary>
		/// <param name="filespace">The filesystem</param>
		/// <param name="unixPath">The unix path of the file to open</param>
		/// <returns>A stream to the file</returns>
		public static Stream OpenFile(this VirtualFileSystem filespace, string unixPath)
		{
			var path=VirtualFileSystem.FromUnixPath(unixPath);
			return filespace.OpenFile(path);
		}

		/// <summary>
		/// Checks to see if a file exists
		/// </summary>
		/// <param name="filespace">The filesystem</param>
		/// <param name="unixPath">The path of the file to check for</param>
		/// <returns>true if the file exists, otherwise false</returns>
		public static bool FileExists(this VirtualFileSystem filespace, string unixPath)
		{
			var path=VirtualFileSystem.FromUnixPath(unixPath);
			return filespace.FileExists(path);
		}

		/// <summary>
		/// Registers a mount point
		/// </summary>
		/// <param name="filespace">The filesystem</param>
		/// <param name="unixPath">The unix path to register the mount point at</param>
		/// <param name="mountPoint">The mount point to register</param>
		public static void RegisterMount(this VirtualFileSystem filespace, string unixPath, IDirectoryNode mountPoint)
		{
			var path=VirtualFileSystem.FromUnixPath(unixPath);
			filespace.RegisterMount(path,mountPoint);
		}

		/// <summary>
		/// Trys to get a mount point
		/// </summary>
		/// <param name="filespace">The filesystem</param>
		/// <param name="unixPath">The unix path to the mount point</param>
		/// <param name="mountPoint">On success the mount point, otherwise null</param>
		/// <returns>true if a mount point was found, otherwise false</returns>
		public static bool TryGetMountPoint(this VirtualFileSystem filespace, string unixPath, out IDirectoryNode mountPoint)
		{
			var path=VirtualFileSystem.FromUnixPath(unixPath);
			return filespace.TryGetMountPoint(path,out mountPoint);
		}

		/// <summary>
		/// Checks to see if a mount point exists
		/// </summary>
		/// <param name="filespace">The filesystem</param>
		/// <param name="unixPath">The path to the mount point to check for</param>
		/// <returns>true if the mount point exists, otherwise false</returns>
		public static bool MountPointExists(this VirtualFileSystem filespace, string unixPath)
		{
			var path=VirtualFileSystem.FromUnixPath(unixPath);

			IDirectoryNode mountPoint=null;
			return filespace.TryGetMountPoint(path,out mountPoint);
		}
	}
}
