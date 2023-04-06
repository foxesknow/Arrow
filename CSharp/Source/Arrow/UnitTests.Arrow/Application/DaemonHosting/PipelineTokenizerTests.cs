using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Application.DaemonHosting;

using NUnit.Framework;

namespace UnitTests.Arrow.Application.DaemonHosting
{
    [TestFixture]
    public class PipelineTokenizerTests
    {
        [Test]
        [TestCase('0', '\0')]
        [TestCase('a', '\a')]
        [TestCase('b', '\b')]
        [TestCase('f', '\f')]
        [TestCase('n', '\n')]
        [TestCase('r', '\r')]
        [TestCase('t', '\t')]
        [TestCase('v', '\v')]
        public void EscapeSequences(char from, char to)
        {
            Assert.That(PipelineTokenizer.ConvertEscape(from), Is.EqualTo(to));
        }

        [Test]
        public void Backtick()
        {
            var items = PipelineTokenizer.Parse("\"``\"").ToList();
            Assert.That(items, Is.Not.Null & Has.Count.EqualTo(1));
            Assert.That(items[0].Value, Is.Not.Null & Is.EqualTo("`"));
        }

        [Test]
        [TestCase('1', '1')]
        [TestCase('\\', '\\')]
        [TestCase('X', 'X')]
        public void EscapeNotSpecial(char from, char to)
        {
            Assert.That(PipelineTokenizer.ConvertEscape(from), Is.EqualTo(to));
        }

        [Test]
        public void EscapeOutsideOfString()
        {
            Assert.Catch(() => PipelineTokenizer.Parse("`n").ToList());
        }

        [Test]
        public void UnterminatedString()
        {
            Assert.Catch(() => PipelineTokenizer.Parse("dir \"hello").ToList());
            Assert.Catch(() => PipelineTokenizer.Parse("dir 'hello").ToList());
        }

        [Test]
        public void IncompleteEscape()
        {
            Assert.Catch(() => PipelineTokenizer.Parse("hello`").ToList());
        }

        [Test]
        public void BadlyFormedPipeline()
        {
            Assert.Catch(() => PipelineTokenizer.Parse(null).ToList());
            Assert.Catch(() => PipelineTokenizer.Parse("|").ToList());
            Assert.Catch(() => PipelineTokenizer.Parse("foo|").ToList());
            Assert.Catch(() => PipelineTokenizer.Parse("|bar").ToList());
            Assert.Catch(() => PipelineTokenizer.Parse("foo|bar|").ToList());
        }

        [Test]
        public void Escape_DoubleQuotes()
        {
            var tokens = PipelineTokenizer.Parse("\"`n\"").ToList();
            Assert.That(tokens.Count, Is.EqualTo(1));

            var token = tokens[0];
            Assert.That(token.TokenType, Is.EqualTo(PipelineTokenizer.TokenType.QuotedText));
            Assert.That(token.Value, Is.EqualTo("\n"));
            Assert.That(token.ToString(), Is.Not.Null & Has.Length.GreaterThan(0));
        }

        [Test]
        public void Escape_SingleQuotes()
        {
            var tokens = PipelineTokenizer.Parse("'`n'").ToList();
            Assert.That(tokens.Count, Is.EqualTo(1));

            var token = tokens[0];
            Assert.That(token.TokenType, Is.EqualTo(PipelineTokenizer.TokenType.QuotedText));
            Assert.That(token.Value, Is.EqualTo("`n"));
        }
    }
}
