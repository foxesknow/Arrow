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
        public void InterpolatedString(LogLevel logLevel)
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
        public void LogTo_LogsToRightLevel(LogLevel logLevel)
        {
            using(var writer = new StringWriter())
            {
                ILog log = new TextWriterLog(writer)
                {
                    AddLogLevel = true,
                    DateTimeMode = DateTimeMode.None
                };

                var sum = 0;
                log.LogTo(logLevel, $"Total = {sum = 1 + 2}");

                var output = writer.ToString().Trim();
                Assert.That(output, Does.StartWith($"[{logLevel}").IgnoreCase);
                Assert.That(sum, Is.EqualTo(3));
            }
        }

        [Test]
        [TestCase(LogLevel.Debug)]
        [TestCase(LogLevel.Info)]
        [TestCase(LogLevel.Warn)]
        [TestCase(LogLevel.Error)]
        [TestCase(LogLevel.Fatal)]
        public void LogTo_LogsNothing(LogLevel logLevel)
        {
            using(var writer = new StringWriter())
            {
                ILog log = new TextWriterLog(writer)
                {
                    AddLogLevel = true,
                    DateTimeMode = DateTimeMode.None,
                    LogLevel = LogLevel.None
                };

                var sum = 0;
                log.LogTo(logLevel, $"Total = {sum = 1 + 2}");

                var output = writer.ToString().Trim();
                Assert.That(output, Is.EqualTo(""));
                Assert.That(sum, Is.EqualTo(0));
            }
        }

        [Test]
        [TestCase(LogLevel.Debug)]
        [TestCase(LogLevel.Info)]
        [TestCase(LogLevel.Warn)]
        [TestCase(LogLevel.Error)]
        [TestCase(LogLevel.Fatal)]
        public void LevelLogging(LogLevel logLevel)
        {
            using(var writer = new StringWriter())
            {
                ILog log = new TextWriterLog(writer)
                {
                    AddLogLevel = true,
                    DateTimeMode = DateTimeMode.None
                };

                var sum = 0;
                switch(logLevel)
                {
                    case LogLevel.Debug:
                        log.Debug($"Total = {sum = 1 + 2}");
                        break;

                    case LogLevel.Info:
                        log.Info($"Total = {sum = 1 + 2}");
                        break;

                    case LogLevel.Warn:
                        log.Warn($"Total = {sum = 1 + 2}");
                        break;

                    case LogLevel.Error:
                        log.Error($"Total = {sum = 1 + 2}");
                        break;

                    case LogLevel.Fatal:
                        log.Fatal($"Total = {sum = 1 + 2}");
                        break;

                    default:
                        throw new Exception();
                }

                var output = writer.ToString().Trim();
                Assert.That(output, Does.StartWith($"[{logLevel}").IgnoreCase);
                Assert.That(sum, Is.EqualTo(3));
            }
        }

        [Test]
        [TestCase(LogLevel.Debug)]
        [TestCase(LogLevel.Info)]
        [TestCase(LogLevel.Warn)]
        [TestCase(LogLevel.Error)]
        [TestCase(LogLevel.Fatal)]
        public void LevelLogging_Disabled(LogLevel logLevel)
        {
            using(var writer = new StringWriter())
            {
                ILog log = new TextWriterLog(writer)
                {
                    AddLogLevel = true,
                    DateTimeMode = DateTimeMode.None,
                    LogLevel = LogLevel.None
                };

                var sum = 0;
                switch(logLevel)
                {
                    case LogLevel.Debug:
                        log.Debug($"Total = {sum = 1 + 2}");
                        break;

                    case LogLevel.Info:
                        log.Info($"Total = {sum = 1 + 2}");
                        break;

                    case LogLevel.Warn:
                        log.Warn($"Total = {sum = 1 + 2}");
                        break;

                    case LogLevel.Error:
                        log.Error($"Total = {sum = 1 + 2}");
                        break;

                    case LogLevel.Fatal:
                        log.Fatal($"Total = {sum = 1 + 2}");
                        break;

                    default:
                        throw new Exception();
                }

                var output = writer.ToString().Trim();
                Assert.That(output, Is.EqualTo(""));
                Assert.That(sum, Is.EqualTo(0));
            }
        }
    }
}
