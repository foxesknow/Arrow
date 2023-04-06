using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Application.Daemon;

using NUnit.Framework;

namespace UnitTests.Arrow.Application.Daemon
{
    [TestFixture]
    public class PipelineSplitterTests
    {
        [Test]
        public void NoLine()
        {
            var pipeline = PipelineSplitter.Split("");
            Assert.That(pipeline, Is.Not.Null & Has.Count.EqualTo(0));
        }

        [Test]
        public void JustWhitespace()
        {
            var pipeline = PipelineSplitter.Split("              ");
            Assert.That(pipeline, Is.Not.Null & Has.Count.EqualTo(0));
        }

        [Test]
        public void EmptyParameter_1()
        {
            var pipeline = PipelineSplitter.Split("dir \"\"");
            Assert.That(pipeline, Is.Not.Null & Has.Count.EqualTo(1));
            
            Assert.That(pipeline[0].Name, Is.EqualTo("dir"));
            Assert.That(pipeline[0].Arguments[0], Is.EqualTo(""));
        }

        [Test]
        public void EmptyParameter_2()
        {
            var pipeline = PipelineSplitter.Split("dir ''");
            Assert.That(pipeline, Is.Not.Null & Has.Count.EqualTo(1));
            
            Assert.That(pipeline[0].Name, Is.EqualTo("dir"));
            Assert.That(pipeline[0].Arguments[0], Is.EqualTo(""));
        }

        [Test]
        public void EscapeSequenceInStringInPipeline_DoubleQuotes()
        {
            var pipeline = PipelineSplitter.Split("dir \"``t\"");
            Assert.That(pipeline, Is.Not.Null & Has.Count.EqualTo(1));
            Assert.That(pipeline[0].Name, Is.EqualTo("dir"));

            Assert.That(pipeline, Is.Not.Null & Has.Count.EqualTo(1));
            Assert.That(pipeline[0].Arguments, Has.Count.EqualTo(1));
            Assert.That(pipeline[0].Arguments[0], Is.EqualTo("`t"));
        }

        [Test]
        public void EscapeSequenceInStringInPipeline_SingleQuotes()
        {
            var pipeline = PipelineSplitter.Split("dir '``t'");
            Assert.That(pipeline, Is.Not.Null & Has.Count.EqualTo(1));
            Assert.That(pipeline[0].Name, Is.EqualTo("dir"));

            Assert.That(pipeline, Is.Not.Null & Has.Count.EqualTo(1));
            Assert.That(pipeline[0].Arguments, Has.Count.EqualTo(1));
            Assert.That(pipeline[0].Arguments[0], Is.EqualTo("``t"));
        }

        [Test]
        public void SinglePart()
        {
            var pipeline = PipelineSplitter.Split("dir *.txt");
            Assert.That(pipeline, Is.Not.Null & Has.Count.EqualTo(1));
            
            var part = pipeline[0];
            Assert.That(part.Name, Is.EqualTo("dir"));
            Assert.That(part.Arguments, Is.Not.Null & Has.Count.EqualTo(1));
            Assert.That(part.Arguments[0], Is.Not.Null & Is.EqualTo("*.txt"));
        }

        [Test]
        public void TwoPart()
        {
            var pipeline = PipelineSplitter.Split(@"dir *.txt | saveto -text c:\temp\log.txt");
            Assert.That(pipeline, Is.Not.Null & Has.Count.EqualTo(2));
            
            var dirPart = pipeline[0];
            Assert.That(dirPart.Name, Is.EqualTo("dir"));
            Assert.That(dirPart.Arguments, Is.Not.Null & Has.Count.EqualTo(1));
            Assert.That(dirPart.Arguments[0], Is.Not.Null & Is.EqualTo("*.txt"));

            var saveToPart = pipeline[1];
            Assert.That(saveToPart.Name, Is.EqualTo("saveto"));
            Assert.That(saveToPart.Arguments, Is.Not.Null & Has.Count.EqualTo(2));
            Assert.That(saveToPart.Arguments[0], Is.Not.Null & Is.EqualTo("-text"));
            Assert.That(saveToPart.Arguments[1], Is.Not.Null & Is.EqualTo(@"c:\temp\log.txt"));
        }

        [Test]
        public void BadlyFormedPipe()
        {
            Assert.Catch(() => PipelineSplitter.Split(null));
            Assert.Catch(() => PipelineSplitter.Split("|"));
            Assert.Catch(() => PipelineSplitter.Split("foo|"));
            Assert.Catch(() => PipelineSplitter.Split("|bar"));
            Assert.Catch(() => PipelineSplitter.Split("foo|bar|"));
        }

        [Test]
        public void SingleItem()
        {
            var parts = PipelineSplitter.Split("dir");
            Assert.That(parts, Has.Count.EqualTo(1));
            Assert.That(parts[0].Name, Is.EqualTo("dir"));
        }

        [Test]
        public void SingleItem_WhitespaceInArgument()
        {
            var parts = PipelineSplitter.Split("dir '    '");
            Assert.That(parts, Has.Count.EqualTo(1));
            Assert.That(parts[0].Name, Is.EqualTo("dir"));
            Assert.That(parts[0].Arguments[0], Is.EqualTo("    "));
        }

        [Test]
        public void SingleItem_WithArguments()
        {
            var parts = PipelineSplitter.Split("dir 1 2 3 \"4 5\" 6 ");
            Assert.That(parts, Has.Count.EqualTo(1));
            Assert.That(parts[0].Name, Is.EqualTo("dir"));
            
            var arguments = parts[0].Arguments;
            Assert.That(arguments, Has.Count.EqualTo(5));
            Assert.That(arguments[0], Is.EqualTo("1"));
            Assert.That(arguments[1], Is.EqualTo("2"));
            Assert.That(arguments[2], Is.EqualTo("3"));
            Assert.That(arguments[3], Is.EqualTo("4 5"));
            Assert.That(arguments[4], Is.EqualTo("6"));
        }

        [Test]
        public void TwoPartsWithEmbeddedSpace()
        {
            var parts = PipelineSplitter.Split("dir    |    save");
            Assert.That(parts, Has.Count.EqualTo(2));
            Assert.That(parts[0].Name, Is.EqualTo("dir"));
            Assert.That(parts[1].Name, Is.EqualTo("save"));
        }

        [Test]
        public void SinglePart_EmbeddedSpace()
        {
            var parts = PipelineSplitter.Split("dir         *.txt");
            Assert.That(parts, Has.Count.EqualTo(1));
            Assert.That(parts[0].Name, Is.EqualTo("dir"));
            Assert.That(parts[0].Arguments[0], Is.EqualTo("*.txt"));
        }

        [Test]
        public void SinglePart_LeadingSpace()
        {
            var parts = PipelineSplitter.Split("         dir");
            Assert.That(parts, Has.Count.EqualTo(1));
            Assert.That(parts[0].Name, Is.EqualTo("dir"));
        }
    }
}
