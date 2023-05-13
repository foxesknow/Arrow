using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Application;

using NUnit.Framework;

namespace UnitTests.Arrow.Application
{
    [TestFixture]
    public class CommandLineSwitchTests
    {
        [Test]
        public void Basics()
        {
            CommandLineSwitch c1 = new CommandLineSwitch("user", "bob");
            Assert.That(c1.Name, Is.EqualTo("user"));
            Assert.IsTrue(c1.Value == "bob");

            // Make sure that swithc names get converted to lowercase
            CommandLineSwitch c2 = new CommandLineSwitch("USER", null);
            Assert.That(c2.Name, Is.EqualTo("user"));
            Assert.IsNull(c2.Value);
        }

        [Test]
        public void TryParse()
        {
            CommandLineSwitch c1 = null;
            Assert.IsTrue(CommandLineSwitch.TryParse("/verbose", out c1));
            Assert.That(c1.Name, Is.EqualTo("verbose"));
            Assert.IsNull(c1.Value);

            CommandLineSwitch c2 = null;
            Assert.IsTrue(CommandLineSwitch.TryParse("-verbose", out c2));
            Assert.That(c2.Name, Is.EqualTo("verbose"));
            Assert.IsNull(c2.Value);

            CommandLineSwitch c3 = null;
            Assert.IsTrue(CommandLineSwitch.TryParse("/user:jack", out c3));
            Assert.That(c3.Name, Is.EqualTo("user"));
            Assert.That(c3.Value, Is.EqualTo("jack"));

            CommandLineSwitch c4 = null;
            Assert.IsFalse(CommandLineSwitch.TryParse("user:jack", out c4));
            Assert.IsNull(c4);
        }

        [Test]
        public void ParseSuccess()
        {
            CommandLineSwitch c1 = CommandLineSwitch.Parse("/verbose");
            Assert.IsNotNull(c1);
            Assert.That(c1.Name, Is.EqualTo("verbose"));
            Assert.IsNull(c1.Value);

            CommandLineSwitch c2 = CommandLineSwitch.Parse("/offset:1");
            Assert.IsNotNull(c2);
            Assert.That(c2.Name, Is.EqualTo("offset"));
            Assert.That(c2.Value, Is.EqualTo("1"));
        }

        [Test]
        public void ParseFail()
        {
            Assert.Throws<FormatException>(() => CommandLineSwitch.Parse("hello"));
        }

        [Test]
        public void EnsureValuePresent_Success()
        {
            CommandLineSwitch c1 = CommandLineSwitch.Parse("/user:jack");
            c1.EnsureValuePresent();
        }

        [Test]
        public void EnsureValuePresent_Fail()
        {
            Assert.Throws<CommandLineSwitchException>(() =>
            {
                CommandLineSwitch c1 = CommandLineSwitch.Parse("/verbose");
                c1.EnsureValuePresent();
            });
        }
    }
}
