using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Common.Services.VirtualDirectory
{
	[Serializable]
	public sealed class DirectoryContentsResponse
	{
		private IReadOnlyList<string> m_Content;

		public DirectoryContentsResponse(IReadOnlyList<string> contents)
		{
			if(contents==null) throw new ArgumentNullException("contents");

			m_Content=contents;
		}

		public IReadOnlyList<string> Contents
		{
			get{return m_Content;}
		}
	}
}
