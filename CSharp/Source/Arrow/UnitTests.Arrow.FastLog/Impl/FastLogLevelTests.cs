
using Arrow.FastLog;
using Arrow.FastLog.Impl;
using NUnit.Framework;

namespace UnitTests.Arrow.FastLog.Impl
{
    [TestFixture]
    public class FastLogLevelTests
    {
        [Test]
        [TestCase(42, "42")]
        [TestCase(0, "0")]
        [TestCase(int.MaxValue, "2147483647")]
        [TestCase(int.MinValue, "-2147483648")]
        public void WriteInt(int value, string expected)
        {
            var logger = new FastLogLevel(null!, LogLevel.Info);
            logger.Write(value);
            Assert.That(logger.LineAndClear(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(42, "x", "2a")]
        [TestCase(42, "x4", "002a")]
        public void WriteIntWithFormat(int value, string format, string expected)
        {
            var logger = new FastLogLevel(null!, LogLevel.Info);
            logger.Write(value, format);
            Assert.That(logger.LineAndClear(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(true, "true")]
        [TestCase(false, "false")]
        public void WriteBool(bool value, string expected)
        {
            var logger = new FastLogLevel(null!, LogLevel.Info);
            logger.Write(value);
            Assert.That(logger.LineAndClear(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(42)]
        public void WriteBool(int age)
        {
            var logger = new FastLogLevel(null!, LogLevel.Info);
            logger.Write($"You are {age}");
            Assert.That(logger.LineAndClear(), Is.EqualTo("You are 42"));
        }
    }
}
