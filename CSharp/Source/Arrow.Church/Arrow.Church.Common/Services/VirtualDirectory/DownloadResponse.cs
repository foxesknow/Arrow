using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Common.Services.VirtualDirectory
{
	[Serializable]
	public sealed class DownloadResponse
	{
		private readonly byte[] m_Data;

		public DownloadResponse(byte[] data)
		{
			if(data==null) throw new ArgumentNullException("data");

			m_Data=data;
		}

		public byte[] Data
		{
			get{return m_Data;}
		}
	}
}
