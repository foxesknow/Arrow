using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Arrow.IO;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.IO
{
	[TestFixture]
	public class ByteSegmentStreamTests
	{
		[Test]
		public void Construction()
		{
			byte[] data=new byte[100];
			
			var stream1=new ByteSegmentStream(data);
			Assert.That(stream1.Position,Is.EqualTo(0));
			Assert.That(stream1.AvailableSpace,Is.EqualTo(data.Length));
			Assert.That(stream1.Length,Is.EqualTo(data.Length));

			var stream2=new ByteSegmentStream(data,10,90);
			Assert.That(stream2.Position,Is.EqualTo(0));
			Assert.That(stream2.AvailableSpace,Is.EqualTo(90));
			Assert.That(stream2.Length,Is.EqualTo(90));

			var stream3=new ByteSegmentStream(new ArraySegment<byte>(data,2,3));
			Assert.That(stream3.Position,Is.EqualTo(0));
			Assert.That(stream3.AvailableSpace,Is.EqualTo(3));
			Assert.That(stream3.Length,Is.EqualTo(3));
		}

		[Test]
		public void Write_FromBegin()
		{
			var data=new byte[20];
			var segment=new ArraySegment<byte>(data);

			using(var stream=new ByteSegmentStream(segment))
			{
				Assert.That(stream.Position,Is.EqualTo(0));
				Assert.That(stream.AvailableSpace,Is.EqualTo(data.Length));

				var toWrite=new Byte[]{1,2,3};
				stream.Write(toWrite,0,toWrite.Length);

				Assert.That(stream.Position,Is.EqualTo(toWrite.Length));
				Assert.That(stream.AvailableSpace,Is.EqualTo(data.Length-toWrite.Length));
			}
		}

		[Test]
		public void Write_FromMid()
		{
			var data=new byte[20];
			var segment=new ArraySegment<byte>(data);

			using(var stream=new ByteSegmentStream(segment))
			{
				Assert.That(stream.Position,Is.EqualTo(0));
				Assert.That(stream.AvailableSpace,Is.EqualTo(data.Length));

				stream.Position=10;

				var toWrite=new Byte[]{1,2,3};
				stream.Write(toWrite,0,toWrite.Length);

				stream.Position=0;
				byte[] readData=new byte[10];

				stream.Read(readData,0,readData.Length);

				for(int i=0; i<readData.Length; i++)
				{
					CheckAllValues(readData,0);
				}
			}
		}

		[Test]
		public void Write_CheckOverflow()
		{
			byte[] data=new byte[200];
			for(int i=0; i<data.Length; i++) data[i]=99;

			using(var stream=new ByteSegmentStream(data,50,100))
			{
				var zeros=new byte[100];
				stream.Write(zeros,0,zeros.Length);

				for(int i=0; i<data.Length; i++)
				{
					if(i>=0 && i<=49) Assert.That(data[i],Is.EqualTo(99));
					if(i>=50 && i<=149) Assert.That(data[i],Is.EqualTo(0));
					if(i>=150 && i<=199) Assert.That(data[i],Is.EqualTo(99));
				}
			}
		}

		[Test]
		public void Read_AllAvailable()
		{
			byte[] data=new byte[200];
			for(int i=0; i<data.Length; i++) data[i]=99;

			using(var stream=new ByteSegmentStream(data))
			{
				byte[] readData=new byte[50];

				int bytesRead=stream.Read(readData,0,readData.Length);
				Assert.That(bytesRead,Is.EqualTo(readData.Length));
				CheckAllValues(readData,99);

				Assert.That(stream.Position,Is.EqualTo(readData.Length));
			}
		}

		[Test]
		public void Read_SomeAvailable()
		{
			byte[] data=new byte[200];
			for(int i=0; i<data.Length; i++) data[i]=99;

			using(var stream=new ByteSegmentStream(data))
			{
				byte[] readData=new byte[250];

				int bytesRead=stream.Read(readData,0,readData.Length);
				Assert.That(bytesRead,Is.EqualTo(data.Length));
				CheckAllValues(readData,0,bytesRead,99);

				Assert.That(stream.Position,Is.EqualTo(data.Length));
			}
		}

		[TestCase(0,10,SeekOrigin.Begin,10)]
		[TestCase(5,3,SeekOrigin.Begin,3)]
		[TestCase(5,3,SeekOrigin.Current,8)]
		[TestCase(20,-3,SeekOrigin.Current,17)]
		[TestCase(100,-12,SeekOrigin.End,88)]
		public void Seek(long basePosition, long offset, SeekOrigin origin, long expectedPosition)
		{
			byte[] data=new byte[100];
			using(var stream=new ByteSegmentStream(data))
			{
				stream.Position=basePosition;
				var newPosition=stream.Seek(offset,origin);

				Assert.That(newPosition,Is.EqualTo(expectedPosition));
				Assert.That(stream.Position,Is.EqualTo(expectedPosition));
			}
		}

		private void CheckAllValues(byte[] data, int expectedByte)
		{
			for(int i=0; i<data.Length; i++)
			{
				Assert.That(data[i],Is.EqualTo(expectedByte));
			}
		}

		private void CheckAllValues(byte[] data, int offset, int count, int expectedByte)
		{
			for(int i=0; i<count; i++)
			{
				Assert.That(data[offset+i],Is.EqualTo(expectedByte));
			}
		}
	}
}
