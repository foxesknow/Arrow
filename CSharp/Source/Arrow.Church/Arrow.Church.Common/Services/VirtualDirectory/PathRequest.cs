using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Common.Services.VirtualDirectory
{
	[Serializable]
	public sealed class PathRequest
	{
		public PathRequest() : this("")
		{
		}

		public PathRequest(string path)
		{
			if(path==null) throw new ArgumentNullException("path");

			this.Path=path.TrimStart();
		}

		public string Path{get; private set;}
	}
}
