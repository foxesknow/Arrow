using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging.Memory
{
	static class MemoryExtensions
	{
		public static string DestinationName(this Uri uri)
		{
			// It's the local path with the leading / removed
			string name=uri.LocalPath;
			return name.Substring(1);
		}
	}
}
