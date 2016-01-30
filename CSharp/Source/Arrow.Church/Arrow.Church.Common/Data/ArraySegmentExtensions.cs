using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Collections;

namespace Arrow.Church.Common.Data
{
	public static class ArraySegmentExtensions
	{
		public static ArraySegment<byte> ToArraySegment(this MemoryStream stream)
		{
			var buffer=stream.GetBuffer();
			return new ArraySegment<byte>(buffer,0,(int)stream.Position);
		}
	}
}
