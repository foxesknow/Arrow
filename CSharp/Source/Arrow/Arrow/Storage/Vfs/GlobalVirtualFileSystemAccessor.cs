using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Storage.Vfs
{
	/// <summary>
	/// Provides accessor access to the global virtual file system
	/// </summary>
	class GlobalVirtualFileSystemAccessor : Accessor
	{
		public static readonly string Scheme="gvfs";
		private static readonly char[] pathSeperator=new char[]{'/'};

		public GlobalVirtualFileSystemAccessor(Uri uri) : base(uri.StripLogonDetails().StripQuery())
		{
			ValidateScheme(uri,Scheme);
		}

		public override Stream OpenRead()
		{
			var uri=this.Uri;

			string filespaceName=uri.Host;
			string[] path=uri.LocalPath.Split(pathSeperator,StringSplitOptions.RemoveEmptyEntries);

			VirtualFileSystem filespace=null;
			if(GlobalVirtualFileSystem.TryGetFilesystem(filespaceName,out filespace)==false)
			{
				throw new IOException("Filespace not registered in global file space: "+filespaceName);
				
			}

			return filespace.OpenFile(path);
		}
	}
}
