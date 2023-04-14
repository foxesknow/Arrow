using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

using Arrow.IO;

using NUnit.Framework;

namespace UnitTests.Arrow.IO
{
    [TestFixture]
    public class SplitStreamTests
    {
        [Test]
        public void Initialization()
        {
            using(var read = new MemoryStream())
            using(var write = new MemoryStream())
            using(var split = new SplitStream(read, write))
            {
                Assert.That(split.ReadStream, Is.SameAs(read));
                Assert.That(split.WriteStream, Is.SameAs(write));

                Assert.That(split.CanRead, Is.True);
                Assert.That(split.CanWrite, Is.True);
                Assert.That(split.CanSeek, Is.False);
            }
        }

        [Test]
        public void ExceptionAfterDispose()
        {
            var stream = MakeStream();
            stream.Dispose();
                
            Assert.Catch(() => stream.WriteByte(8));
        }

        [Test]
        public void ReadByte()
        {
            using(var stream = MakeStream())
            {
                stream.ReadStream.Write(new byte[]{5, 10, 15, 20}, 0, 4);
                stream.ReadStream.Position = 0;

                var read = stream.ReadByte();
                Assert.That(read, Is.EqualTo(5));

                read = stream.ReadByte();
                Assert.That(read, Is.EqualTo(10));
            }
        }

        [Test]
        public void Read()
        {
            using(var stream = MakeStream())
            {
                stream.ReadStream.Write(new byte[]{5, 10, 15, 20}, 0, 4);
                stream.ReadStream.Position = 0;

                var buffer = new byte[2];
                var bytesRead = stream.Read(buffer, 0, 2);
                Assert.That(bytesRead, Is.EqualTo(2));
                Assert.That(buffer[0], Is.EqualTo(5));
                Assert.That(buffer[1], Is.EqualTo(10));
            }
        }

        [Test]
        public async Task ReadAsync()
        {
            using(var stream = MakeStream())
            {
                stream.ReadStream.Write(new byte[]{5, 10, 15, 20}, 0, 4);
                stream.ReadStream.Position = 0;

                var buffer = new byte[2];
                var bytesRead = await stream.ReadAsync(buffer, 0, 2);
                Assert.That(bytesRead, Is.EqualTo(2));
                Assert.That(buffer[0], Is.EqualTo(5));
                Assert.That(buffer[1], Is.EqualTo(10));
            }
        }

        [Test]
        public void WriteByte()
        {
            using(var stream = MakeStream())
            {
                Assert.That(stream.WriteStream.Position, Is.EqualTo(0));

                stream.WriteByte(1);
                Assert.That(stream.WriteStream.Position, Is.EqualTo(1));
            }
        }

        [Test]
        public void Write()
        {
            using(var stream = MakeStream())
            {
                Assert.That(stream.WriteStream.Position, Is.EqualTo(0));

                var buffer = new byte[]{5, 10, 15, 20};
                stream.Write(buffer, 0, buffer.Length);
                Assert.That(stream.WriteStream.Position, Is.EqualTo(buffer.Length));
            }
        }

        [Test]
        public async Task WriteAsync()
        {
            using(var stream = MakeStream())
            {
                Assert.That(stream.WriteStream.Position, Is.EqualTo(0));

                var buffer = new byte[]{5, 10, 15, 20};
                await stream.WriteAsync(buffer, 0, buffer.Length);
                Assert.That(stream.WriteStream.Position, Is.EqualTo(buffer.Length));
            }
        }

        [Test]
        public void Flush()
        {
            using(var stream = MakeStream())
            {
                Assert.That(stream.WriteStream.Position, Is.EqualTo(0));

                var buffer = new byte[]{5, 10, 15, 20};
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
                Assert.That(stream.WriteStream.Position, Is.EqualTo(buffer.Length));
            }
        }

        [Test]
        public async Task FlushAsync()
        {
            using(var stream = MakeStream())
            {
                Assert.That(stream.WriteStream.Position, Is.EqualTo(0));

                var buffer = new byte[]{5, 10, 15, 20};
                await stream.WriteAsync(buffer, 0, buffer.Length);
                await stream.FlushAsync();
                Assert.That(stream.WriteStream.Position, Is.EqualTo(buffer.Length));
            }
        }

        private SplitStream MakeStream()
        {
            var read = new MemoryStream();
            var write = new MemoryStream();
            
            return new SplitStream(read, write);
        }
    }
}
