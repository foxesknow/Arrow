using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using Arrow.IO;

using NUnit.Framework;

namespace UnitTests.Arrow.IO
{
    [TestFixture]
    public class ArrayPoolMemoryStreamTests
    {
        [Test]
        public void Initialize_BadCapacity()
        {
            Assert.Catch(() => new ArrayPoolMemoryStream(-1));
        }

        [Test]
        public void Initialize()
        {
            using(var stream = new ArrayPoolMemoryStream())
            {
                Assert.That(stream.Position, Is.EqualTo(0));
                Assert.That(stream.Length, Is.EqualTo(0));
            }
        }

        [Test]
        public void WriteByte()
        {
            const byte BytesToWrite = 203;

            using(var stream = new ArrayPoolMemoryStream())
            {
                for(byte i = 0; i < BytesToWrite; i++)
                {
                    stream.WriteByte(i);
                    Assert.That(stream.Position, Is.EqualTo(i + 1));
                    Assert.That(stream.Length, Is.EqualTo(i + 1));
                }

                Assert.That(stream.Length, Is.EqualTo(BytesToWrite));

                using(var d = stream.Detach())
                {
                    Assert.That(d.HasBuffer, Is.True);
                    Assert.That(d.Length, Is.EqualTo(BytesToWrite));

                    var span = d.AsSpan();

                    for(byte i = 0; i < BytesToWrite; i++)
                    {
                        Assert.That(span[i], Is.EqualTo(i));
                    }
                }

                // As we've detached the stream should have reset
                Assert.That(stream.Position, Is.EqualTo(0));
                
                using(var d = stream.Detach())
                {
                    Assert.That(d.HasBuffer, Is.True);
                    Assert.That(d.Length, Is.EqualTo(0));
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(GenerateStrings))]
        public void Write_ReadOnlySpan(string text)
        {
            ReadOnlySpan<byte> data = Encoding.UTF8.GetBytes(text);

            using(var stream = new ArrayPoolMemoryStream())
            {
                stream.Write(data);
                Assert.That(stream.Position, Is.EqualTo(data.Length));

                using(var d = stream.Detach())
                {
                    using(var memStream = new MemoryStream(d.Buffer, 0, d.Length))
                    using(var reader = new StreamReader(memStream))
                    {
                        var line = reader.ReadToEnd();
                        Assert.That(line, Is.EqualTo(text));
                    }
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(GenerateStrings))]
        public void Write(string text)
        {
            var data = Encoding.UTF8.GetBytes(text);

            using(var stream = new ArrayPoolMemoryStream())
            {
                stream.Write(data, 0, data.Length);
                Assert.That(stream.Position, Is.EqualTo(data.Length));

                using(var d = stream.Detach())
                {
                    using(var memStream = new MemoryStream(d.Buffer, 0, d.Length))
                    using(var reader = new StreamReader(memStream))
                    {
                        var line = reader.ReadToEnd();
                        Assert.That(line, Is.EqualTo(text));
                    }
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(GenerateStrings))]
        public async Task WriteAsync(string text)
        {
            var data = Encoding.UTF8.GetBytes(text);

            using(var stream = new ArrayPoolMemoryStream())
            {
                await stream.WriteAsync(data, 0, data.Length);
                Assert.That(stream.Position, Is.EqualTo(data.Length));

                using(var d = stream.Detach())
                {
                    using(var memStream = new MemoryStream(d.Buffer, 0, d.Length))
                    using(var reader = new StreamReader(memStream))
                    {
                        var line = reader.ReadToEnd();
                        Assert.That(line, Is.EqualTo(text));
                    }
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(GenerateStrings))]
        public async Task WriteAsync_ReadOnlyMemory(string text)
        {
            ReadOnlyMemory<byte> data = Encoding.UTF8.GetBytes(text);

            using(var stream = new ArrayPoolMemoryStream())
            {
                await stream.WriteAsync(data);
                Assert.That(stream.Position, Is.EqualTo(data.Length));

                using(var d = stream.Detach())
                {
                    using(var memStream = new MemoryStream(d.Buffer, 0, d.Length))
                    using(var reader = new StreamReader(memStream))
                    {
                        var line = reader.ReadToEnd();
                        Assert.That(line, Is.EqualTo(text));
                    }
                }
            }
        }

        [Test]
        public void Detach_NothingWritten()
        {
            using(var stream = new ArrayPoolMemoryStream())
            {
                using(var d = stream.Detach())
                {
                    // There will always be a buffer, but it's logical length will be 0
                    Assert.That(d.HasBuffer, Is.True);
                    Assert.That(d.Buffer, Is.Not.Null);
                    Assert.That(d.Length, Is.EqualTo(0));

                    var span = d.AsSpan();
                    Assert.That(span.Length, Is.EqualTo(0));

                    var memory = d.AsMemory();
                    Assert.That(memory.Length, Is.EqualTo(0));
                }
            }
        }

        [Test]
        public void DetachedBuffer_DefaultState()
        {
            var d = new ArrayPoolMemoryStream.DetachedBuffer();
            Assert.That(d.HasBuffer, Is.False);
            Assert.That(d.Buffer, Is.Null);
            Assert.That(d.Length, Is.EqualTo(0));

            var span = d.AsSpan();
            Assert.That(span.Length, Is.EqualTo(0));

            var memory = d.AsMemory();
            Assert.That(memory.Length, Is.EqualTo(0));
        }

        private static IEnumerable<string> GenerateStrings()
        {
            yield return "";
            yield return "A";
            yield return "Foo";
            yield return "Hello, world";
            
            yield return string.Join("", Enumerable.Repeat("ABcdeFGHijkLMNopqrsTUVwxyz", 100));

            yield return "Goodbye";
        }
    }
}
