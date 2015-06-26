using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common.Data;
using ProtoBuf;

namespace Arrow.Church.Protobuf.Common
{
    /// <summary>
    /// Protobuf serialization class
    /// </summary>
	public class ProtobufMessageProtocol : MessageProtocol
    {
		public override object FromBuffer(byte[] buffer, Type expectedType)
		{
			using(var stream=new MemoryStream(buffer,false))
			{
				return FromStream(stream,expectedType);
			}
		}

		public override object FromStream(Stream stream, Type expectedType)
		{
			return Serializer.NonGeneric.Deserialize(expectedType,stream);
		}

		public override byte[] ToBuffer(object @object)
		{
			using(var stream=new MemoryStream(2048))
			{
				ToStream(stream,@object);
				return stream.ToArray();
			}
		}

		public override void ToStream(Stream stream, object @object)
		{
			Serializer.NonGeneric.Serialize(stream,@object);
		}
	}
}
