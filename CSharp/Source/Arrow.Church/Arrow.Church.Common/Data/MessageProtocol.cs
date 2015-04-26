using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Common.Data
{
	public abstract class MessageProtocol
	{
		public abstract object FromBuffer(byte[] buffer);
		public abstract object FromStream(Stream stream);

		public abstract byte[] ToBuffer(object @object);
		public abstract void ToStream(Stream stream, object @object);
	}
}
