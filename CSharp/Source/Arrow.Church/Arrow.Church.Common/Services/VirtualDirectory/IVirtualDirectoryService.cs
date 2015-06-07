using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common.Data.DotNet;

namespace Arrow.Church.Common.Services.VirtualDirectory
{
	[ChurchService("VirtualDirectory",typeof(SerializationMessageProtocol))]
	public interface IVirtualDirectoryService
	{
		Task<DirectoryContentsResponse> GetFiles(PathRequest request);
		Task<DirectoryContentsResponse> GetDirectories(PathRequest request);
		Task<DownloadResponse> Download(PathRequest request);
	}
}
