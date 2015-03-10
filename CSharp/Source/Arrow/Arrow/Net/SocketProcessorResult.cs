using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Net
{
	public class SocketProcessorResult<THeader,TBody>
	{
		private readonly THeader m_Header;
		private readonly TBody m_Body;

		public SocketProcessorResult(THeader header, TBody body)
		{
			m_Header=header;
			m_Body=body;
		}

		public THeader Header
		{
			get{return m_Header;}
		}

		public TBody Body
		{
			get{return m_Body;}
		}
	}
}
