using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Serialization;

namespace Arrow.Church.Common.Data.DotNet
{
	/// <summary>
	/// Implements .NET serialization
	/// </summary>
	public class SerializationMessageProtocol : MessageProtocol
	{
		public override object FromBuffer(byte[] buffer, Type expectedType)
		{
			return GenericBinaryFormatter.FromArray<object>(buffer);
		}

		public override object FromStream(Stream stream, Type expectedType)
		{
			return GenericBinaryFormatter.FromStream<object>(stream);
		}

		public override byte[] ToBuffer(object @object)
		{
			return GenericBinaryFormatter.ToArray(@object);
		}

		public override void ToStream(Stream stream, object @object)
		{
			GenericBinaryFormatter.ToStream(stream,@object);
		}
	}
}
