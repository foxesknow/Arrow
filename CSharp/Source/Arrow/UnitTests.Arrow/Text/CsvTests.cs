using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Text;

using NUnit.Framework;

namespace UnitTests.Arrow.Text
{
    [TestFixture]
    public class CsvTests
    {
        [Test]
        public void EmptyString()
        {
            Assert.That(Csv.Escape(""), Is.EqualTo(""));
        }

        [Test]
        public void SingleCharacter()
        {
            Assert.That(Csv.Escape("A"), Is.EqualTo("A"));
        }

        [Test]
        public void SingleComma()
        {
            Assert.That(Csv.Escape(","), Is.EqualTo("\",\""));
        }

        [Test]
        public void SingleQuote()
        {
            Assert.That(Csv.Escape("\""), Is.EqualTo("\"\""));
        }

        [Test]
        public void HasComma()
        {
            Assert.That(Csv.Escape("Hello, world"), Is.EqualTo("\"Hello, world\""));
        }

        [Test]
        public void TwoQuote()
        {
            Assert.That(Csv.Escape("\"\""), Is.EqualTo("\"\"\"\""));
        }

        [Test]
        public void HasEmbeddedQuote()
        {
            Assert.That(Csv.Escape("Hello \"Jack\" how are you"), Is.EqualTo("Hello \"\"Jack\"\" how are you"));
        }

        [Test]
        public void HasEmbeddedQuoteAndComman()
        {
            Assert.That(Csv.Escape("Hello \"Jack\", how are you"), Is.EqualTo("\"Hello \"\"Jack\"\", how are you\""));
        }
    }
}
