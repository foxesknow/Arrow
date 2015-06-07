using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Arrow.IO;
using Arrow.Church.Common.Services.VirtualDirectory;

namespace Arrow.Church.Server.Services.VirtualDirectory
{
	public class VirtualDirectoryService : ChurchService<IVirtualDirectoryService>, IVirtualDirectoryService
	{
		private SandboxDirectory m_Sandbox;

		public string Root
		{
			get{return m_Sandbox.Root;}
			set
			{
				m_Sandbox=new SandboxDirectory(value);
			}
		}

		private string Normalize(string directory)
		{
			if(m_Sandbox==null) throw new IOException("no sandbox root specified");

			return m_Sandbox.Normalize(directory);
		}

		Task<DirectoryContentsResponse> IVirtualDirectoryService.GetFiles(PathRequest request)
		{
			if(request==null) throw new ArgumentNullException("request");

			string path=Normalize(request.Path);
			var files=Directory.GetFiles(path);

			return Task.FromResult(new DirectoryContentsResponse(files));
		}

		Task<DirectoryContentsResponse> IVirtualDirectoryService.GetDirectories(PathRequest request)
		{
			if(request==null) throw new ArgumentNullException("request");

			string path=Normalize(request.Path);
			var directories=Directory.GetDirectories(path);

			return Task.FromResult(new DirectoryContentsResponse(directories));
		}

		async Task<DownloadResponse> IVirtualDirectoryService.Download(PathRequest request)
		{
			if(request==null) throw new ArgumentNullException("request");

			string path=Normalize(request.Path);
			using(var stream=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read,4096,true))
			{
				var length=stream.Length;

				byte[] buffer=new byte[length];
				await stream.ReadAsync(buffer,0,(int)length);

				return new DownloadResponse(buffer);
			}
		}
	}
}
