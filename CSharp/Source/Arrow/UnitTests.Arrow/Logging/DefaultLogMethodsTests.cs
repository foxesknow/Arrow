using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using Arrow.Logging;
using Arrow.Logging.Loggers;
using NUnit.Framework;

namespace UnitTests.Arrow.Logging
{
    [TestFixture]
    public class DefaultLogMethodsTests
    {
        [Test]
        [TestCase(LogLevel.Debug)]
        [TestCase(LogLevel.Info)]
        [TestCase(LogLevel.Warn)]
        [TestCase(LogLevel.Error)]
        [TestCase(LogLevel.Fatal)]
        public void FormattableStringSupport(LogLevel logLevel)
        {
            using(var writer = new StringWriter())
            {
                ILog log = new TextWriterLog(writer);
                log.LogTo(logLevel, $"Total = {1 + 2}");

                var output = writer.ToString().Trim();
                Assert.That(output, Is.EqualTo("Total = 3"));
            }
        }

        [Test]
        [TestCase(LogLevel.Debug)]
        [TestCase(LogLevel.Info)]
        [TestCase(LogLevel.Warn)]
        [TestCase(LogLevel.Error)]
        [TestCase(LogLevel.Fatal)]
        public void FormattableStringSupport_LogsToRightLevel(LogLevel logLevel)
        {
            using(var writer = new StringWriter())
            {
                ILog log = new TextWriterLog(writer)
                {
                    AddLogLevel = true,
                    DateTimeMode = DateTimeMode.None
                };

                log.LogTo(logLevel, $"Total = {1 + 2}");

                var output = writer.ToString().Trim();
                Assert.That(output, Does.StartWith($"[{logLevel}").IgnoreCase);
            }
        }
    }
}
